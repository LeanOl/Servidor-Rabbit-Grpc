using AppServer.Domain;

namespace AppServer.Data;

public class ProductDatabase
{
    private static ICollection<Product> _products = new List<Product>();
    private static int _id = 0;
    private static ProductDatabase? _instance = null;
    private static readonly object _lock = new object();

    public static ProductDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProductDatabase();
                    }
                }
            }
            return _instance;
        }
    }

    public IEnumerable<Product> Get(Func<Product, bool> condition)
    {
        lock (_lock)
        {
            return _products.Where(condition);
        }
    }

    public IEnumerable<Product> GetAll()
    {
        lock (_lock)
        {
            return _products;
        }
    }

    public void Add(Product product)
    {
        lock (_lock)
        {
            if (_products.Any(p => p.Name == product.Name
                && p.Description == product.Description))
            {
                throw new Exception("El producto ya existe");
            }
            product.Id = _id;
            _id++;
            _products.Add(product);
        }
    }

    public void Modify(Product product)
    {
        lock (_lock)
        {
            var productToModify = _products.FirstOrDefault(p => p.Id == product.Id);
            if (productToModify != null)
            {
                productToModify.Name = product.Name;
                productToModify.Description = product.Description;
                productToModify.Stock = product.Stock;
                productToModify.Price = product.Price;
                productToModify.Image = product.Image;
            }
        }
    }

    public void Delete(Product product)
    {
        lock (_lock)
        {
            _products.Remove(product);
        }
    }
}
