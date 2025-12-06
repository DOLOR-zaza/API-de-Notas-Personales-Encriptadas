using System.Security.Cryptography;
using System.Text;

namespace API_BACKEND1.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        // 32 bytes = AES-256
        private readonly byte[] _key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        // 16 bytes para IV
        private readonly byte[] _iv  = Encoding.UTF8.GetBytes("abcdefghijklmnop");

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            var encryptedBytes = ms.ToArray();
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }
}
