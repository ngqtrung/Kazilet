using System;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace PRN231_Kazilet_API.Utils
{
    public class Common
    {
        private static readonly Random random = new Random();
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        internal string GeneratePassword(int length = 12)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            // Ensure password includes at least one character from each category
            StringBuilder password = new StringBuilder();
            password.Append(lower[random.Next(lower.Length)]);
            password.Append(upper[random.Next(upper.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(symbols[random.Next(symbols.Length)]);

            // Combine all characters for the rest of the password
            string allCharacters = lower + upper + digits + symbols;

            for (int i = 4; i < length; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            // Convert to array and shuffle to ensure randomness
            char[] passwordArray = password.ToString().ToCharArray();
            ShuffleArray(passwordArray);

            return new string(passwordArray);
        }

        private static void ShuffleArray(char[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
