using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
namespace AdminServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        static string grpcAddress = System.Configuration.ConfigurationManager.AppSettings["GrpcAddress"];

        private Admin.AdminClient client;

        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> PostUser([FromBody] ProductDTO product)
        {
            using var channel = GrpcChannel.ForAddress(grpcAddress);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostProductAsync(product);
            return Ok(reply.Message);
        }

        [HttpPut]
        public async Task<ActionResult> PutUser([FromBody] ModifyProductRequest product)
        {
            using var channel = GrpcChannel.ForAddress(grpcAddress);
            client = new Admin.AdminClient(channel);
            var reply = await client.ModifyProductAsync(product);
            return Ok(reply.Message);
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteProductRequest product)
        {
            using var channel = GrpcChannel.ForAddress(grpcAddress);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteProductAsync(product);
            return Ok(reply.Message);
        }

        [HttpGet("reviews/{id}")]
        public async Task<ActionResult> GetReviews([FromRoute] GetReviewsRequest id)
        {
            using var channel = GrpcChannel.ForAddress(grpcAddress);
            client = new Admin.AdminClient(channel);
            var reply = await client.GetReviewsAsync(id);
            return Ok(reply.Reviews);
        }





    }
}