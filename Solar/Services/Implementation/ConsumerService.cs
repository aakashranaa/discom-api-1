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
            var excelReader = new ExcelReader();
            List<Record> _records = excelReader.ReadExcel("ut-crest-rep.xlsx");
            // var subDiv = await this._dbContext.GetAreaNameByAreaIdAsync(10);

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
                    status_code = "600",
                    message = "Invalid Key",
                    consumerNo = consumerId
                };
            }

            // var response = await GetDiscomApplicantData(consumerId);
            try
            {
                var response = _records.FirstOrDefault(r => r.consumer_id == consumerId);
                if (response != null)
                {
                    var consumerResponse = new Consumer200Response
                    {
                        status_code = response.status_code,
                        consumer_address = response.consumer_address,
                        consumer_lg_district_code = response.consumer_lg_district_code,
                        consumer_pin_code = response.consumer_pin_code,
                        connection_load = response.connection_load,
                        circle_name = response.circle_name,
                        circle_code = response.circle_code,
                        division_name = response.division_name,
                        division_code = response.division_code,
                        sub_division_name = response.sub_division_name,
                        sub_division_code = response.sub_division_code,
                        connection_type = response.connection_type,
                        consumer_mobile = response.consumer_mobile,
                        consumer_email = response.consumer_email,
                        consumer_name = response.consumer_name,
                        existing_installed_capacity = response.existing_installed_capacity
                    };

                    var responseInString = JsonConvert.SerializeObject(consumerResponse);
                    var encryptedResponse = EncryptAESCBC(responseInString);

                    return encryptedResponse;
                }

                else
                {
                    return new ConsumerOtherResponse
                    {
                        status_code = "300",
                        message = "Data not available",
                        consumerNo = consumerId
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConsumerOtherResponse
                {
                    status_code = "800",
                    message = "Server Unavailable",
                    consumerNo = consumerId
                };
            }

        }
    

        public async Task<string> GetDivisionNameFromSubDivisionNameAsync(string subDivisionName)
        {
            // Query the tblArea table to get the division name (AreaName) based on the sub-division name
            var divisionName = await _dbContext.tblArea
                .Where(a => a.AreaName == subDivisionName) // Filter by sub-division name
                .Select(a => a.pAreaID.HasValue ? _dbContext.tblArea.FirstOrDefault(d => d.AreaID == a.pAreaID.Value).AreaName : null) // Get the division name (AreaName) based on the parentAreaId (pAreaID)
                .FirstOrDefaultAsync();

            return divisionName;
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
                    var subDivisionName = (await this._dbContext.GetAreaNameByAreaIdAsync(discomApplicationFormData.PlantAreaID ?? 0)).AreaName.Trim();
                    var divisionName = await this.GetDivisionNameFromSubDivisionNameAsync(subDivisionName);
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
                        circle_name = "Chandigarh",
                        circle_code = discomApplicationFormData.CircleCode?.ToString() ?? "",
                        consumer_address = $"{(!string.IsNullOrEmpty(applicantData.HNo) ? applicantData.HNo + ", " : "")}{(!string.IsNullOrEmpty(applicantData.StreetName) ? "Street - " + applicantData.StreetName + ", " : "")}{(!string.IsNullOrEmpty(applicantData.VillageName) ? "Village - " + applicantData.VillageName + ", " : "")}{(!string.IsNullOrEmpty(applicantData.MandalName) ? "MandalName - " + applicantData.MandalName : "")}".TrimEnd(',', ' '),

                        connection_type = (discomApplicationFormData.PhaseType != null && discomApplicationFormData.PhaseType.Contains("Single") ? 1 : 2).ToString() ?? "",
                        division_name = divisionName?.Trim() ?? "",
                        sub_division_code = discomApplicationFormData.SubDivisionCode?.ToString().Trim(),
                        sub_division_name = subDivisionName.Trim() ?? "",
                        consumer_lg_district_code = applicantData.DistrictId.ToString()?.Trim() // hardcoded for chandigarh
                    };
                }
                else
                {
                    return new ConsumerOtherResponse
                    {
                        status_code = "300",
                        message = "Data not available",
                        consumerNo = consumerId
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConsumerOtherResponse
                {
                    status_code = "800",
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
