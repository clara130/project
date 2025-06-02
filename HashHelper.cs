using System;
using System.Security.Cryptography;
using System.Text;

namespace proj
{
    public static class HashHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder result = new StringBuilder();
                foreach (byte b in bytes)
                {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString();
            }
        }

        public static bool ComparePasswords(string plainPassword, string hashedPassword)
        {
            string hashedInput = HashPassword(plainPassword);
            return hashedInput == hashedPassword;
        }
    }
}
