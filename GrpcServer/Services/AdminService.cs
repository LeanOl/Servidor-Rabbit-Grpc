using AppServer.Domain;
using AppServer.Services;
using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services
{
    public class AdminService : Admin.AdminBase
    {
        public override Task<MessageReply> PostProduct(ProductDTO request, ServerCallContext context)
        {
           ProductManager productManager = new ProductManager();
           string message;
            try
            {
                productManager.PublishProduct(request.Name, request.Description, request.Stock, request.Price, request.Username);
                 message = $"Producto {request.Name} publicado correctamente";
            }
            catch (Exception e)
            {
               message = $"Error al publicar el producto: {e.Message}";
            }

            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> ModifyProduct(ModifyProductRequest request, ServerCallContext context)
        {
            ProductManager productManager = new ProductManager();
            string message;
            try
            {
                productManager.ModifyProductData(request.Id, request.Username, request.Description, request.Stock,
                    request.Price);
                message = $"Producto {request.Id} modificado correctamente";
            }
            catch (Exception e)
            {
                message = $"Error al modificar el producto: {e.Message}";
            }
            
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            ProductManager productManager = new ProductManager();
            string message;
            try
            {
                productManager.DeleteProduct(request.Id, request.Username);
                message = $"Producto {request.Id} eliminado correctamente";
            }
            catch (Exception e)
            {
                message = $"Error al eliminar el producto: {e.Message}";
            }
            return Task.FromResult(new MessageReply { Message = message });
        }
        public override Task<MessageReply> BuyProduct(BuyProductRequest request, ServerCallContext context)
        {
            ProductManager productManager = new ProductManager();
            string message;
            try
            {
                productManager.BuyProduct(request.Id);
                message = $"Producto {request.Id} se ha comprado correctamente";
            }
            catch (Exception e)
            {
                message = $"Error al comprar el producto: {e.Message}";
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<GetReviewsResponse> GetReviews(GetReviewsRequest request, ServerCallContext context)
        {
            ProductManager productManager = new ProductManager();
            Product product = productManager.GetProduct(request.Id);
            List<ReviewDTO> reviews = new List<ReviewDTO>();
            foreach (var review in product.Reviews)
            {
                ReviewDTO reviewDTO = new ReviewDTO
                {
                    Rating = review.Rating,
                    Comment = review.Comment
                };
                reviews.Add(reviewDTO);
            }
            GetReviewsResponse response = new GetReviewsResponse();
            response.Reviews.AddRange(reviews);
            return Task.FromResult(response);
        }


    }
}