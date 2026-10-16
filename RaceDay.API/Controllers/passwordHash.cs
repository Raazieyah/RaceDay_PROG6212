using System.Security.Cryptography;
using System.Text;

//step 13 aunthetication-(Password is hashed) 
public static class PasswordHasher
{
	public  static string HashPassword (string password)
	{
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}
