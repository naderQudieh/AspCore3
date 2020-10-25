using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AppZeroAPI.Services
{
    public class EncryptorService
    {
        public string HashPassword(string password, string salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var bytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(bytes);
        }

        public string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private readonly byte[] _keyBytes =
        {
            243, 211, 148, 241, 45, 56, 204, 72, 98, 129, 85, 9, 121, 233, 195, 236,
            202, 178, 210, 224, 188, 126, 134, 131, 67, 230, 232, 72, 42, 239, 191, 84
        };
        private readonly byte[] _ivBytes = { 180, 230, 246, 6, 138, 15, 193, 244, 76, 122, 254, 225, 236, 198, 17, 189 };

        public string Encrypt(string stringToEncrypt)
        {
            byte[] encryptedToken;
            using (var aes = Aes.Create())
            {
                aes.Key = _keyBytes;
                aes.IV = _ivBytes;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var mStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(stringToEncrypt);
                        }
                        encryptedToken = mStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encryptedToken);
        }

        public string Decrypt(string encryptedString)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(encryptedString);
                string decryptedToken;
                using (var aes = Aes.Create())
                {
                    aes.Key = _keyBytes;
                    aes.IV = _ivBytes;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var mStream = new MemoryStream(tokenBytes))
                    {
                        using (var decryptoStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(decryptoStream))
                            {
                                decryptedToken = streamReader.ReadToEnd();
                            }
                        }
                    }
                }

                return decryptedToken;
            }
            catch (Exception)
            {
                throw new Exception("Invalid Token");
            }
        }

        public string GetSecretKey()
        {
            return Convert.ToBase64String(new System.Security.Cryptography.HMACSHA256().Key);
        }
    }
}