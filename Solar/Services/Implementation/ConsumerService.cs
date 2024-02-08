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
        private readonly string encryptionKey = "hJ5sD8FpWb2cR4oA3VqG9xZtJy7uX6wK";

        public async Task<object> GetConsumer(string encryptedData)
        {
            Console.WriteLine("heree");

            var subDiv = await this._dbContext.GetAreaNameByAreaIdAsync(10);

            // Convert the base64 string to a byte array
            byte[] base64Data = Convert.FromBase64String(encryptedData);
            // var decryptedData = DecryptData(base64Data);

            var decryptedData = DecryptAESCBC(encryptedData);

            // need to fetch consumerId from decrypted data
            var consumerId = JsonConvert.DeserializeObject<ExpectedConsumerPayload>(decryptedData)?.ConsumerNo;

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

            var responseInString = JsonConvert.SerializeObject(response);

            var encryptedResponse = EncryptAESCBC(responseInString);

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
                        division_code = discomApplicationFormData.DivisionCode?.Trim() ?? "",
                        circle_name = "",
                        circle_code = discomApplicationFormData.CircleCode?.ToString() ?? "",
                        consumer_address = applicantData.Address ?? "",
                        connection_type = (discomApplicationFormData.PhaseType != null && discomApplicationFormData.PhaseType.Contains("Single") ? 1 : 2).ToString() ?? "",
                        division_name = "",
                        sub_division_code = discomApplicationFormData.SubDivisionCode?.ToString().Trim(),
                        sub_division_name = (await this._dbContext.GetAreaNameByAreaIdAsync(discomApplicationFormData.PlantAreaID ?? 0)).AreaName,
                        consumer_lg_district_code = applicantData.DistrictId.ToString()?.Trim() // hardcoded for chandigarh
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

        public string EncryptAESCBC(string text)
        {
            string Key = encryptionKey;
            string IV = encryptionKey.Substring(0, 16);
            byte[] plaintextbytes = System.Text.ASCIIEncoding.ASCII.GetBytes(text);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //   aes.Padding = PaddingMode.Zeros;
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
            aes.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encrypted = crypto.TransformFinalBlock(plaintextbytes, 0, plaintextbytes.Length);
            crypto.Dispose();
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptAESCBC(string encrypted)
        {
            string Key = encryptionKey;
            string IV = encryptionKey.Substring(0, 16);
            byte[] encryptedbytes = Convert.FromBase64String(encrypted);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            // aes.Padding = PaddingMode.Zeros;
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
            aes.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] secret = crypto.TransformFinalBlock(encryptedbytes, 0, encryptedbytes.Length);
            crypto.Dispose();
            return System.Text.ASCIIEncoding.ASCII.GetString(secret);
        }


        public string EncryptData(object data)
        {
            var str = JsonConvert.SerializeObject(data);
            return EncryptAESCBC(str);
            // Serialize data to JSON
            /*var jsonData = JsonConvert.SerializeObject(data);

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
            */


        }

        public string DecryptData(string encryptedData)
        {

            return DecryptAESCBC(encryptedData);
            /*using (Aes aesAlg = Aes.Create())
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
            }*/
        }
    }

    public class ExpectedConsumerPayload
    {
        public string ConsumerNo { get; set; }   
    }
}
