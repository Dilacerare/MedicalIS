using Automarket.Domain.Entity;
using Automarket.Domain.Enum;
using Automarket.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Automarket.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Car> Cars { get; set; }
        
        public DbSet<Profile> Profiles { get; set; }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Basket> Baskets { get; set; }
        
        public DbSet<Order> Orders { get; set; }
        
        public DbSet<PerfectHealth> Healths { get; set; }
        
        public DbSet<Recommendation> Recommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("Users").HasKey(x => x.Id);
                
                builder.HasData(new User[]
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Admin",
                        Password = HashPasswordHelper.HashPassowrd("123456"),
                        Role = Role.Admin
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Moderator",
                        Password = HashPasswordHelper.HashPassowrd("654321"),
                        Role = Role.Moderator
                    }
                });

                builder.Property(x => x.Id).ValueGeneratedOnAdd();

                builder.Property(x => x.Password).IsRequired();
                builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

                builder.HasOne(x => x.Profile)
                    .WithOne(x => x.User)
                    .HasPrincipalKey<User>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);
                
                builder.HasOne(x => x.Basket)
                    .WithOne(x => x.User)
                    .HasPrincipalKey<User>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);
                
            });
            
            modelBuilder.Entity<Car>(builder =>
            {
                builder.ToTable("Cars").HasKey(x => x.Id);
                
                builder.HasData(new Car
                {
                    Id = 1,
                    Name = "ITHomester",
                    Description = new string('A', 50),
                    DateCreate = DateTime.Now,
                    Speed = 230,
                    Model = "BMW",
                    Avatar = null,
                    TypeCar = TypeCar.PassengerCar
                });
            });

            modelBuilder.Entity<Profile>(builder =>
            {
                builder.ToTable("Profiles").HasKey(x => x.Id);
                
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.Age);
                builder.Property(x => x.Address).HasMaxLength(200).IsRequired(false);
                builder.Property(x => x.Temperature);
                builder.Property(x => x.BloodPressure).HasMaxLength(200).IsRequired(false);
                builder.Property(x => x.GUrineAnalysis).HasMaxLength(200).IsRequired(false);
                builder.Property(x => x.GBloodTest).HasMaxLength(200).IsRequired(false);
                builder.Property(x => x.Cholesterol);
                
                builder.HasData(new Profile()
                {
                    Id = 1,
                    UserId = 1
                });
                
                builder.HasMany(e => e.Recommendations)
                    .WithOne(e => e.Patient)
                    .HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Basket>(builder =>
            {
                builder.ToTable("Baskets").HasKey(x => x.Id);
                
                builder.HasData(new Basket() 
                {
                    Id = 1,
                    UserId = 1
                });
            });
            
            modelBuilder.Entity<Order>(builder =>
            {
                builder.ToTable("Orders").HasKey(x => x.Id);

                builder.HasOne(r => r.Basket).WithMany(t => t.Orders)
                    .HasForeignKey(r => r.BasketId);
            });
            
            modelBuilder.Entity<PerfectHealth>(builder =>
            {
                builder.ToTable("PerfectHealths").HasKey(x => x.Id);
                
                builder.HasData(new PerfectHealth
                {
                    Id = 1,
                    Temperature = 36.6,
                    BloodPressure = "120/60",
                    GUrineAnalysis = "Норма",
                    GBloodTest = "Норма",
                    Cholesterol = 5.2
                });
            });
            
            modelBuilder.Entity<Recommendation>(builder =>
            {
                builder.ToTable("Recommendation").HasKey(x => x.Id);
                
                builder.HasData(new Recommendation
                {
                    Id = 1,
                    ProfileId = 1,
                    Author = "Admin",
                    Description = "Живи"
                });
            });
        }
    }
}