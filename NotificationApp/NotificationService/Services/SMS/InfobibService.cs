using Infobip.Api.SDK;
using Infobip.Api.SDK.SMS.Models;
using NotificationService.Services;

namespace NotificationService;

public class InfobibService : ISmsService
{
    public async Task<bool> SendSmsAsync(string to, string message)
    {
        try
        {
            var configuration = new ApiClientConfiguration(
                "https://pev2j8.api.infobip.com",
                "secret");

            var client = new InfobipApiClient(configuration);

            var request = new SendSmsMessageRequest()
            {
                Messages = new List<SmsMessage>()
                {
                    new SmsMessage()
                    {
                        Destinations = new List<SmsDestination>()
                        {
                            new SmsDestination(Guid.NewGuid().ToString(), "966535939521")
                        },
                        From = "ServiceSMS",
                        Text = "ddddddddd"
                    }
                },
            };
            await client.Sms.SendSmsMessage(request);
            return true;

        }
        catch (Exception e)
        {
            return false;
        }
    }
}