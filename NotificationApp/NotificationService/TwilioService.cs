namespace NotificationService;

using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioService: ITwilioService
{
    private readonly string accountSid = "rs";
    private readonly string authToken = "rs";
    private readonly string twilioPhoneNumber = "+s";

    public TwilioService()
    {
        TwilioClient.Init(accountSid, authToken);
    }

    public void SendSms(string to, string message)
    {
        var messageResponse = MessageResource.Create(
           new PhoneNumber("+18777804236"),
            body: message,
            from: new PhoneNumber(twilioPhoneNumber)
        );

        Console.WriteLine($"Message sent with SID: {messageResponse.Sid}");
    }
}
