using Microsoft.AspNetCore.Mvc;
using Solar.Services.Abstraction;

namespace Solar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumerController : ControllerBase
    {

        private readonly ILogger<ConsumerController> _logger;
        private readonly IConsumerService _consumerService;

        public ConsumerController(ILogger<ConsumerController> logger, IConsumerService consumerService)
        {
            _logger = logger;
            _consumerService = consumerService;
        }

        [HttpGet("encrypt")]
        public IActionResult GetEncryptData(string consumerId)
        {
            Console.WriteLine(consumerId);
            var payload = new
            {
                consumerNo = consumerId
            };
            var data = this._consumerService.EncryptData(payload);
            return Ok(data);
        }

        [HttpPost("decrypt")]
        public IActionResult GetDecryptedData([FromBody] Payload payload)
        {
            string res = payload.payload;
            // Convert the base64 string to a byte array
            // byte[] base64Data = Convert.FromBase64String(res);
            var response = this._consumerService.DecryptData(res);
            return Ok(response);
        }

        [HttpPost("consumer-detail")]
        public async Task<IActionResult> GetConsumerDetails([FromBody] Payload payload)
        {
            var encryptedData = payload.payload;
            Console.WriteLine(encryptedData);
            var data = await this._consumerService.GetConsumer(encryptedData);
            return Ok(data);
        }
    }

    public class Payload
    {
        public string payload { get; set; }
    }
}
