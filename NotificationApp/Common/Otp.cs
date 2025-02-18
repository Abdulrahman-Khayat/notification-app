using System;
using System.Security.Cryptography;
using System.Text;

public static class Otp
{
    private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    public static string GenerateOtp()
    {
        byte[] randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        var otp = BitConverter.ToInt32(randomBytes, 0) % 1000000;
        return Math.Abs(otp).ToString("D6"); // Ensures a 6-digit OTP
    }

    private static string HashOtp(string otp, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(otp));
        return Convert.ToBase64String(hash);
    }

    public static bool ValidateOtp(string enteredOtp, string storedHash, string secret)
    {
        var hashedOtp = HashOtp(enteredOtp, secret);
        return hashedOtp == storedHash;
    }
}