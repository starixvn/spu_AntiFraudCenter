namespace AntiFraudCenterService.Shares
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptographicHelper
    {
        public static string AESEncrypt(string plainText, string serectKey, string participantName)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(serectKey);
            byte[] saltBytes = Encoding.UTF8.GetBytes(string.Concat(participantName, serectKey));

            string encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = Constants.Aes128;
                aes.BlockSize = Constants.Aes128;
                aes.Key = keyBytes;
                aes.IV = saltBytes.Take(aes.BlockSize / 8).ToArray();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(cs))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    encrypted = Convert.ToBase64String(ms.ToArray());
                }
            }

            return encrypted;
        }

        public static string AESDecrypt(string cipherString, string serectKey, string participantName)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(serectKey);
            byte[] saltBytes = Encoding.UTF8.GetBytes(string.Concat(participantName, serectKey));

            var cipherBytes = Convert.FromBase64String(cipherString);

            string plaintext = string.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = Constants.Aes128;
                aes.BlockSize = Constants.Aes128;
                aes.Key = keyBytes;
                aes.IV = saltBytes.Take(aes.BlockSize / 8).ToArray();

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static string GetRandomHexNumber(int numberOfByte)
        {
            var csprng = new RNGCryptoServiceProvider();

            var bytes = new byte[numberOfByte];

            csprng.GetNonZeroBytes(bytes);

            return string.Join("", bytes.Select(b => b.ToString("x2")));
        }
    }
}