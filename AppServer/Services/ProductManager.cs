using System.Text;
using AppServer.Data;
using AppServer.Domain;

namespace AppServer.Services;

public class ProductManager
{
    ProductDatabase _productDatabase = ProductDatabase.Instance;
    public bool PublishProduct(Byte[] product)
    {
        try
        {
            string productString = Encoding.UTF8.GetString(product);
            string[] productArray = productString.Split(":");
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
            _productDatabase.Add(newProduct);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        
    }
}