namespace Common;

public interface IOTPService
{
    public string GenerateOTP(Guid userId);
    public bool ValidateOTP(Guid userId, string inputOtp);
    public void DeleteOTP(Guid userId);
}