using AppServer.Data;
using AppServer.Domain;
using Protocol;
using System.Configuration;

namespace AppServer.Services;

public class ProductManager
{
    ProductDatabase _productDatabase = ProductDatabase.Instance;
    public async Task PublishProductAsync(string product,FileCommsHandler fileCommsHandler)
    {
        string[] productArray = product.Split(Constant.Separator1);
            string name = productArray[0];
            string description = productArray[1];
            int stock = Convert.ToInt32(productArray[2]);
            int price = Convert.ToInt32(productArray[3]);
            string username = productArray[4];
            
            

            Product newProduct = new Product
                {
                    Name = name,
                    Description = description,
                    Stock = stock,
                    Price = price,
                    Owner = username
                    
                };
            string imagePath = ConfigurationManager.AppSettings[ServerConfig.imagePath];
            string imageName = newProduct.Id + "-image.jpg";
            string filePath = await fileCommsHandler.ReceiveFileAsync(imagePath, imageName);
            newProduct.Image = filePath;
            try
            {
                _productDatabase.Add(newProduct);
            }
            catch (Exception e)
            {
                File.Delete(filePath);
                throw new Exception(e.Message);
            }

    }

    public string GetProducts(string name)
    {
        string productString = "";
        if (name == "")
        {
            IEnumerable<Product> products = _productDatabase.GetAll();
            foreach (var product in products)
            {
                if(productString!="")
                    productString += Constant.Separator2;
                productString += product.Id+Constant.Separator1+product.Name+Constant.Separator1+product.Description+Constant.Separator1+product.Stock ;

            }
        }
        else
        {
            IEnumerable<Product> products = _productDatabase.Get(p => p.Name == name).ToList();
            foreach (var product in products)
            {
                if(productString!="")
                    productString += Constant.Separator2; 

                productString+= product.Id + Constant.Separator1 + product.Name + Constant.Separator1 + product.Description + Constant.Separator1 + product.Stock;
            }
        }

        return productString;
    }

    public void BuyProduct(int idProduct,string username)
    {
       
        Product product = _productDatabase.Get(p => p.Id == idProduct).FirstOrDefault();
        if (product != null )
        {
            if(product.Stock == 0)
                throw new Exception("Error! el producto no tiene stock");
            product.Stock--;
            Purchase purchase = new Purchase
            {
                ProductId = idProduct,
                Username = username,
                Date = DateTime.Now
            };
            EventSender.SendPurchaseEvent(purchase);

        }else
        { 
            throw new Exception("Error! el producto no existe");
        }
        
    }

    public (string,string) GetSpecificProduct(string productId)
    {
        try
        {
            int id = Convert.ToInt32(productId);
            Product product = _productDatabase.Get(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                throw new Exception("Error! el producto no existe");
            }
            string productReviews="";
            foreach (var review in product.Reviews )
            {
                if(productReviews!="")
                    productReviews += Constant.Separator2;
                productReviews += $"{review.Rating}{Constant.Separator3}{review.Comment}";
            }
            
            string productString = Constant.OkCode+ Constant.Separator1 +
                                   product.Id + Constant.Separator1 +
                                   product.Name + Constant.Separator1 +
                                   product.Description + Constant.Separator1 +
                                   product.Stock + Constant.Separator1 +
                                   product.Price + Constant.Separator1 +
                                   product.Owner + Constant.Separator1 +
                                   productReviews;
            return (productString,product.Image);
        }
        catch (Exception e)
        {
            throw new Exception($"{Constant.ErrorCode}{Constant.Separator1}Error! el producto no existe");
        }
    }

    public void AddReview(string productId)
    {
        string[] productArray = productId.Split(Constant.Separator1);
        int id = Convert.ToInt32(productArray[0]);
        int rating = Convert.ToInt32(productArray[1]);
        string comment = productArray[2];
        Product product = _productDatabase.Get(p => p.Id == id).FirstOrDefault();
        if (product == null)
            throw new Exception("Error! el producto no existe");
        product.Reviews.Add(new Review{Comment = comment,Rating = rating});
    }

    public void DeleteProduct(int productId,string username)
    {
        Product product = _productDatabase.Get(p => p.Id == productId).FirstOrDefault();
        if (product == null)
            throw new Exception("Error! el producto no existe");
        if (product.Owner != username)
            throw new Exception("Error! el producto no pertenece al usuario");
        FileDeleter.DeleteFile(product.Image);
        _productDatabase.Delete(product);
    }

    public void ModifyProductData(int productId, string username, string description,int stock, int price)
    {
        Product productToModify = _productDatabase.Get(p => p.Id == productId).FirstOrDefault();
        if (productToModify == null)
            throw new Exception($"{Constant.ErrorCode}{Constant.Separator1}Error! el producto no existe");
        if (productToModify.Owner != username)
            throw new Exception($"{Constant.ErrorCode}{Constant.Separator1}Error! el producto no pertenece al usuario");
        productToModify.Description = description;
        productToModify.Stock = stock;
        productToModify.Price = price;
    }

    public async Task ModifyProductImage(int productId,string username, FileCommsHandler fileCommsHandler)
    {
        
        Product product = _productDatabase.Get(p => p.Id == productId).FirstOrDefault();
        if (product == null)
            throw new Exception("Error! el producto no existe");
        if (product.Owner != username)
            throw new Exception("Error! el producto no pertenece al usuario");
        FileDeleter.DeleteFile(product.Image);
        string fileName = productId + "-image.jpg";
        await fileCommsHandler.ReceiveFileAsync(ConfigurationManager.AppSettings[ServerConfig.imagePath], fileName);
    }

   
    public void PublishProduct(string name, string description, int stock, int price, string username)
    {

        Product newProduct = new Product
        {
            Name = name, 
            Description = description, 
            Stock = stock,
            Price = price, Owner = username
        };

        _productDatabase.Add(newProduct);

    }

    public Product GetProduct(int productId)
    {
        Product product = _productDatabase.Get(p => p.Id == productId).FirstOrDefault();
       if(product == null)
           throw new Exception("Error! el producto no existe");
       return product;
    }
}