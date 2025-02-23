using Infobip.Api.SDK;
using Infobip.Api.SDK.SMS.Models;

namespace NotificationService;

public class InfobibService : IInfobip
{
    public async Task<SendSmsMessageResponse> SendSms()
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
        return await client.Sms.SendSmsMessage(request);
    }
}