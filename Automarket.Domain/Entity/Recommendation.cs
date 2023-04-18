namespace Automarket.Domain.Entity;

public class Recommendation
{
    public long Id { get; set; }
    
    public string Author { get; set; }
    
    public Profile Patient { get; set; }
    
    public long ProfileId { get; set; }

    public string Description { get; set; }
    
    public DateTime DateCreate { get; set; }
}