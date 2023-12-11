using PurchasesServer.Domain;

namespace PurchasesServer.Storage;

public class PurchaseDB
{
    private ICollection<Purchase> _purchases = new List<Purchase>();

    private static PurchaseDB _instance;

    public static PurchaseDB Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PurchaseDB();
            }
            return _instance;
        }
    }

    public void AddPurchase(Purchase purchase)
    {
        _purchases.Add(purchase);
        Console.WriteLine($"Added purchase: {purchase.Username} bought {purchase.ProductId} at {purchase.Date}");
    }

    public IEnumerable<Purchase> GetFilteredPurchases(PurchaseFilters filters)
    {
        IEnumerable<Purchase> filteredPurchases = _purchases;

        if (filters.Username != null)
            filteredPurchases = filteredPurchases.Where(p => p.Username == filters.Username);

        if (filters.ProductId != null)
            filteredPurchases = filteredPurchases.Where(p => p.ProductId == filters.ProductId);

        if (filters.Date != null)
        {
            
                var date = filters.Date.Split('/');
                var day = int.Parse(date[0]);
                var month = int.Parse(date[1]);
                var year = int.Parse(date[2]);
                filteredPurchases = filteredPurchases.Where(p => p.Date.Day == day && p.Date.Month == month && p.Date.Year == year);
           
        }
            


        return filteredPurchases;
    }

}