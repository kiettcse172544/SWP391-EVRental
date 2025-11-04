using System.Security.Cryptography;
using System.Text;

public static class PasswordTools
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public static bool VerifyPassword(string input, string storedHash)
    {
        var check = HashPassword(input);
        return string.Equals(check, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
