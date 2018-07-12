using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassLibrary1
{
    class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FooContext>
    {
        public FooContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<FooContext>()
                .UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=Foo;Trusted_Connection=True;")
                .Options;
            return new FooContext(options);
        }
    }
}