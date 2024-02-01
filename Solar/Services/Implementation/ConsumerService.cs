using Microsoft.EntityFrameworkCore;
using Solar.Services.Abstraction;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using Solar.Db.Tables;
using Solar.Models;
using Solar.Controllers;

namespace Solar.Services.Implementation
{
    public class ConsumerService(DiscomDbContext dbContext) : IConsumerService
    {
        private readonly DiscomDbContext _dbContext = dbContext;
        private readonly string encryptionKey = "constant_key_that_is_32_characte";

        public async Task<object> GetConsumer(string encryptedData)
        {
            // Convert the base64 string to a byte array
            byte[] base64Data = Convert.FromBase64String(encryptedData);
            var decryptedData = DecryptData(base64Data);

            // need to fetch consumerId from decrypted data
            var consumerId = JsonConvert.DeserializeObject<ExpectedConsumerPayload>(decryptedData)?.ConsumerId;

            if (consumerId == null)
            {
                return new ConsumerOtherResponse
                {
                    status = "600",
                    message = "Invalid Key",
                    consumerNo = consumerId
                };
            }

            var response = await GetDiscomApplicantData(consumerId);

            var encryptedResponse = EncryptData(response);

            return encryptedResponse;
        }


        async public Task<object> GetDiscomApplicantData(string consumerId)
        {
            try
            {
                var applicantData = await _dbContext.tblApplicant
                    .Where(x => x.ConsumerNo == consumerId).FirstOrDefaultAsync();

                var discomApplicationFormData = await _dbContext.tblDiscomApplicationForm.Where(x => x.ApplicantID == applicantData.ApplicantID).FirstOrDefaultAsync();

                if (applicantData != null && discomApplicationFormData != null)
                {

                    return new Consumer200Response()
                    {
                        status_code = "200",
                        consumer_mobile = (applicantData.Mobile ?? "").Trim(),
                        consumer_email = (applicantData.EmailId ?? "").Trim(),
                        consumer_pin_code = (applicantData.Pin ?? "").Trim(),
                        connection_load = discomApplicationFormData.ConnectedLoad?.ToString() ?? "",
                        consumer_name = $"{applicantData.IndividualFirstName} {applicantData.IndividualLastName}" ?? "",
                        existing_installed_capacity = discomApplicationFormData.EarlierInstalledCapacity?.ToString() ?? "",
                        division_code = discomApplicationFormData.DivisionCode ?? "",
                        circle_name = "",
                        circle_code = discomApplicationFormData.CircleCode?.ToString() ?? "",
                        consumer_address = applicantData.Address ?? "",
                        connection_type = (discomApplicationFormData.PhaseType != null && discomApplicationFormData.PhaseType.Contains("Single") ? 1 : 2).ToString() ?? "",
                        division_name = "",
                        sub_division_code = discomApplicationFormData.SubDivisionCode?.ToString(),
                        sub_division_name = "",
                        consumer_lg_district_code = "44" // hardcoded for chandigarh
                    };
                }
                else
                {
                    return new ConsumerOtherResponse
                    {
                        status = "300",
                        message = "Data not available",
                        consumerNo = consumerId
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConsumerOtherResponse
                {
                    status = "800",
                    message = "Server Unavailable",
                    consumerNo = consumerId
                };
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

    public class ExpectedConsumerPayload
    {
        public string ConsumerId { get; set; }   
    }
}
