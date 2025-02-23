using NotificationService.Services;

namespace NotificationService;

using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioService: ISmsService
{
    private readonly string accountSid = "rs";
    private readonly string authToken = "rs";
    private readonly string twilioPhoneNumber = "+s";

    public TwilioService()
    {
        TwilioClient.Init(accountSid, authToken);
    }

    public async Task<bool> SendSmsAsync(string to, string message)
    {
        try
        {
            var messageResponse = await MessageResource.CreateAsync(
               new PhoneNumber("+18777804236"),
                body: message,
                from: new PhoneNumber(twilioPhoneNumber)
            );
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
