using Infobip.Api.SDK.SMS.Models;

namespace NotificationService;

public interface IInfobip
{
    public Task<SendSmsMessageResponse> SendSms();
}