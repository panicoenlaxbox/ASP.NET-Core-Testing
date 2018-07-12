using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1
{
    public class FooContext : DbContext
    {
        public FooContext(DbContextOptions<FooContext> options) : base(options)
        {

        }
        public DbSet<Foo> Foo { get; set; }
    }
}