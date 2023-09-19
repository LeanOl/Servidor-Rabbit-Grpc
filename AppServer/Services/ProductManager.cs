using System.Text;
using AppServer.Data;
using AppServer.Domain;
using Protocol;

namespace AppServer.Services;

public class ProductManager
{
    ProductDatabase _productDatabase = ProductDatabase.Instance;
    public void PublishProduct(string product,string imagePath)
    {
        
            string[] productArray = product.Split(":");
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
                Owner = username,
                Image = imagePath
            };
            
            _productDatabase.Add(newProduct);
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
                    productString += Protocol.Constant.Separator2;
                productString += product.Id+Protocol.Constant.Separator1+product.Name+Protocol.Constant.Separator1+product.Description+Protocol.Constant.Separator1+product.Stock ;

            }
        }
        else
        {
            IEnumerable<Product> products = _productDatabase.Get((p) => p.Name == name).ToList();
            foreach (var product in products)
            {
                if(productString!="")
                    productString += Protocol.Constant.Separator2; 

                productString= product.Id + Protocol.Constant.Separator1 + product.Name + Protocol.Constant.Separator1 + product.Description + Protocol.Constant.Separator1 + product.Stock;
            }
        }

        return productString;
    }

    public void BuyProduct(string id)
    {
        int idProduct = Convert.ToInt32(id);
        Product product = _productDatabase.Get((p) => p.Id == idProduct).FirstOrDefault();
        if (product != null )
        {
            if(product.Stock == 0)
                throw new Exception("Error! el producto no tiene stock");
            product.Stock--;
        }else
        { 
            throw new Exception("Error! el producto no existe");
        }
        
    }
}