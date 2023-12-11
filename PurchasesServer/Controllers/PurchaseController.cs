using Microsoft.AspNetCore.Mvc;
using PurchasesServer.Domain;
using PurchasesServer.Storage;

namespace PurchasesServer.Controllers
{
    [ApiController]
    [ExceptionFilter]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(ILogger<PurchaseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Purchase> GetPurchases([FromQuery]int? productId, [FromQuery]string? username, [FromQuery]string? date)
        {

            return PurchaseDB.Instance.GetFilteredPurchases(new PurchaseFilters
            {
                ProductId = productId,
                Username = username,
                Date = date
            });
        }
    }
}