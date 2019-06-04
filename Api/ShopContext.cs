using Api.Entities;
using Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}