using Microsoft.EntityFrameworkCore;
using Solar.Services.Abstraction;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;

namespace Solar.Services.Implementation
{
    public class ConsumerService(DiscomDbContext dbContext) : IConsumerService
    {
        private readonly DiscomDbContext _dbContext = dbContext;
        private readonly string encryptionKey = "constant_key_that_is_32_characte";

        public async Task<object> GetConsumer(string consumerId)
        {
            var solarApplication = await _dbContext.tblState.ToListAsync();

            if (solarApplication != null)
            {
                var encryptedData = EncryptData(solarApplication);
                return encryptedData;
            }
            else
            {
                // Handle the case when the consumerId is not found
                return null;
            }
        }


        public string EncryptData(object data)
        {
            // Serialize data to JSON
            var jsonData = JsonConvert.SerializeObject(data);

            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 16));

                // Step 1: Use AES-256-CBC encryption
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Step 2: Encrypt JSON data
                            swEncrypt.Write(jsonData);
                        }
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptData(byte[] encryptedBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 16));

                // Step 4: Use AES-256-CBC decryption
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Decrypt and return the JSON data
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
