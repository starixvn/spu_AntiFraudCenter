namespace AntiFraudCenterService.Shares
{
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptographicHelper
    {
        public static byte[] ÂesEncrypt(string plainText, string serectKey, string participantName)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(serectKey);
            byte[] saltBytes = Encoding.UTF8.GetBytes(string.Concat(participantName, serectKey));

            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = Constants.Aes128;
                aes.BlockSize = Constants.Aes128;
                aes.Key = keyBytes;
                aes.IV = saltBytes.Take(aes.BlockSize / 8).ToArray();

                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new(cs))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    encrypted = ms.ToArray();
                }
            }

            return encrypted;
        }

        public static string AesDecrypt(byte[] cipherBytes, string serectKey, string participantName)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(serectKey);
            byte[] saltBytes = Encoding.UTF8.GetBytes(string.Concat(participantName, serectKey));

            string plaintext = string.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = Constants.Aes128;
                aes.BlockSize = Constants.Aes128;
                aes.Key = keyBytes;
                aes.IV = saltBytes.Take(aes.BlockSize / 8).ToArray();

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static string GetRandomHexNumber()
        {
            using var csprng = new RNGCryptoServiceProvider();
            var bytes = new byte[8];

            csprng.GetNonZeroBytes(bytes);

            return string.Join("", bytes.Select(b => b.ToString("x2")));
        }
    }
}