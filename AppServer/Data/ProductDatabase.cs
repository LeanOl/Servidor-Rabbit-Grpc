using AppServer.Domain;

namespace AppServer.Data;

public class ProductDatabase
{
    private static ICollection<Product> _products = new List<Product>();
    private static ProductDatabase? _instance;

    public static ProductDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProductDatabase();
            }
            return _instance;
        }
    }

    public IEnumerable<Product> Get(Func<bool> condition)
    {
        return _products.Where(p => condition());
    }

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public void Modify(Product product)
    {
        var productToModify = _products.FirstOrDefault(p => p.Id == product.Id);
        if (productToModify != null)
        {
            productToModify.Name = product.Name;
            productToModify.Description = product.Description;
            productToModify.Stock = product.Stock;
            productToModify.Price = product.Price;
            productToModify.Imagen = product.Imagen;
        }
    }

    public void Delete(Product product)
    {
        _products.Remove(product);
    }
}