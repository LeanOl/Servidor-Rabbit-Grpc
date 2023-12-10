namespace AppServer.Domain;

public class Purchase
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Username { get; set; }
    public DateTime Date { get; set; }
}