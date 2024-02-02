namespace Solar.Services.Abstraction
{
    public interface IConsumerService
    {
        Task<object> GetConsumer(string consumerId);

        string EncryptData(object data);

        string DecryptData(string encryptedBytes);
    }
}
