 
using NRedisStack;
using StackExchange.Redis;
using System;

namespace Common;

public class OTPService: IOTPService
{
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _database;

    public OTPService(ConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    // Method to generate OTP if one doesn't already exist in Redis
    public string GenerateOTP(Guid userId, string type)
    {
        var otpKey = $"{type}:otp:{userId}";

        // Check if OTP already exists in Redis
        if (_database.KeyExists(otpKey))
        {
            // Return a message indicating that OTP already exists
            return null;
        }

        // Generate random OTP if not already in Redis
        var otp = new Random().Next(100000, 999999).ToString();

        // Store OTP in Redis with a 5-minute expiration time (300 seconds)
        _database.StringSet(otpKey, otp, TimeSpan.FromMinutes(5));

        return otp;  // Return OTP to be sent to user
    }

    // Method to validate OTP from Redis
    public bool ValidateOTP(Guid userId, string inputOtp, string type)
    {
        var otpKey = $"{type}:otp:{userId}";

        // Retrieve OTP from Redis
        var storedOtp = _database.StringGet(otpKey);

        if (!storedOtp.HasValue)
        {
            // OTP expired or not found
            return false;
        }

        // Validate if the input OTP matches the stored OTP
        return storedOtp.ToString() == inputOtp;
    }

    // Optionally, method to delete OTP after use (to prevent reuse)
    public void DeleteOTP(Guid userId, string type)
    {
        var otpKey = $"{type}:otp:{userId}";
        _database.KeyDelete(otpKey);
    }
}
