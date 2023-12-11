namespace PurchasesServer.Domain;

public class Purchase
{
    public int ProductId { get; set; }
    public string Username { get; set; }
    public DateTime Date { get; set; }
}