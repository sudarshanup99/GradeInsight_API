using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace GradeInsight.Utilities
{
    public class HashingAndVerification
    {
        
            public enum Supported_HA
            {
                SHA256
            }
            public static string ComputeHash(string plainText, Supported_HA hash, byte[]? salt)
            {

                int minSaltLength = 16, maxSaltLength = 16;
                byte[]? saltBytes;
                if (salt != null)
                {
                    saltBytes = salt;
                }
                else
                {
                    Random r = new();
                    int SaltLength = r.Next(minSaltLength, maxSaltLength);
                    saltBytes = new byte[SaltLength];
                    var rng = RandomNumberGenerator.Create();
                    rng.GetNonZeroBytes(saltBytes);
                    rng.Dispose();
                }
                byte[] plainData = ASCIIEncoding.UTF8.GetBytes(plainText);
                byte[] plainDataWithSalt = new byte[plainData.Length + saltBytes.Length];
                for (int x = 0; x < plainData.Length; x++)
                    plainDataWithSalt[x] = plainData[x];
                for (int n = 0; n < saltBytes.Length; n++)
                    plainDataWithSalt[plainData.Length + n] = saltBytes[n];
                byte[]? hashValue = null;
                switch (hash)
                {
                    case Supported_HA.SHA256:
                        var sha = SHA256.Create();
                        hashValue = sha.ComputeHash(plainDataWithSalt);
                        sha.Dispose();
                        break;
                }
                byte[] result = new byte[hashValue!.Length + saltBytes.Length];
                for (int x = 0; x < hashValue.Length; x++)
                    result[x] = hashValue[x];
                for (int n = 0; n < saltBytes.Length; n++)
                    result[hashValue.Length + n] = saltBytes[n];
                return Convert.ToBase64String(result);
            }
            public static bool Confirm(string plainText, string hashValue, Supported_HA hash)
            {

                byte[] hashBytes = Convert.FromBase64String(hashValue);
                int hashSize = 0;
                switch (hash)
                {
                    case Supported_HA.SHA256:
                        hashSize = 32;
                        break;
                }
                byte[] saltBytes = new byte[hashBytes.Length - hashSize];
                for (int x = 0; x < saltBytes.Length; x++)
                    saltBytes[x] = hashBytes[hashSize + x];
                string newHash = ComputeHash(plainText, hash, saltBytes);
                return (hashValue == newHash);
            }
            public static string EncodeTo64(string toEncode)
            {
                byte[] toEncodeAsBytes
                      = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
                string returnValue
                      = System.Convert.ToBase64String(toEncodeAsBytes);
                return returnValue;
            }
            public static void ValidatePassword(string password)
            {
                string patternPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).{6,12}$";
                if (!string.IsNullOrEmpty(password))
                {
                    if (!Regex.IsMatch(password, patternPassword))
                    {
                        throw new Exception("Password must be at least 6 characters, no more than 12 characters, and must include at least one upper case letter, one lower case letter, one special character and one numeric digit.");
                    }
                }
            }
        }
    }

