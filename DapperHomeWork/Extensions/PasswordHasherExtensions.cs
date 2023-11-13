namespace DapperHomeWork.Extensions;

public static class PasswordHasherExtensions
{
    public static string HashPassword(this string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(this string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}