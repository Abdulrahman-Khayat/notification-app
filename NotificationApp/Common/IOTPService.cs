namespace Common;

public interface IOTPService
{
    public string GenerateOTP(Guid userId, string type);
    public bool ValidateOTP(Guid userId, string inputOtp, string type);
    public void DeleteOTP(Guid userId, string type);
}