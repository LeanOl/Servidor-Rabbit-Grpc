using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        static string grpcAddress = System.Configuration.ConfigurationManager.AppSettings["GrpcAddress"];

        private Admin.AdminClient client;

        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(ILogger<PurchaseController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> PostPurchase([FromBody] BuyProductRequest productId)
        {
            using var channel = GrpcChannel.ForAddress(grpcAddress);
            client = new Admin.AdminClient(channel);
            var reply = await client.BuyProductAsync(productId);
            return Ok(reply.Message);
        }
    }
}
