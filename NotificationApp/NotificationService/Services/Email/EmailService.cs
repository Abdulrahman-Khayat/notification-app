using System.Net;
using System.Net.Mail;
using Polly;
using NotificationService.Services;
using Polly.Retry;

public class EmailService: IEmailService
{
    private readonly IConfiguration _config;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<EmailService> _logger;
    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
            
        _retryPolicy = Policy
            .Handle<SmtpException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"Attempt {retryCount}: Email sending failed. Retrying in {timeSpan.TotalSeconds} seconds...");
                });
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        var SmtpClient1 = new SmtpClient(_config["EmailSettings:SmtpServer#1"])
        {
            Port = int.Parse(_config["EmailSettings:SmtpPort#1"]),
            EnableSsl = false
        };

        var SmtpClient2 = new SmtpClient(_config["EmailSettings:SmtpServer#2"])
        {
            Port = int.Parse(_config["EmailSettings:SmtpPort#2"]),
            EnableSsl = false
        };
        
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["EmailSettings:SenderEmail"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);
     
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await SmtpClient1.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully using SMTP#1.");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"SMTP#1 failed after 3 retries. Switching to SMTP#2. Error: {ex.Message}");

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await SmtpClient2.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully using SMTP#2.");
                });
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError($"Both SMTP servers failed. Email not sent. Error: {fallbackEx.Message}");
                return false;
            }
        }
        return true;
    }
}