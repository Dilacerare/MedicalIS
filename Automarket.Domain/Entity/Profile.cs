namespace Automarket.Domain.Entity
{
    public class Profile
    {
        public long Id { get; set; }
        
        public byte Age { get; set; }
        
        public string Address { get; set; }
        
        public long UserId { get; set; }
        
        public User User { get; set; }
        
        public double Temperature { get; set; }
        
        public string BloodPressure { get; set; }
        
        public string GUrineAnalysis { get; set; }
        
        public string GBloodTest { get; set; }
        
        public double Cholesterol { get; set; }
        
        public ICollection<Recommendation> Recommendations { get; set; }
    }
}