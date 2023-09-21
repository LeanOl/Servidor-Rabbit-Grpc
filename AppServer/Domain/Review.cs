namespace AppServer.Domain;

public class Review
{
    private int _rating;

    public string Comment { get; set; }

    public int Rating
    {
        get { return _rating; }
        set
        {
            if (value < 1 || value > 10)
            {
                throw new ArgumentOutOfRangeException("ERROR! La calificacion debe estar entre 1 y 10");
            }
            _rating = value;
        }
    }
}
