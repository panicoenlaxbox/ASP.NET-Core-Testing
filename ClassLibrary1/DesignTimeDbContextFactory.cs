﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassLibrary1
{
    class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShopContext>
    {
        public ShopContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ShopContext>()
                .UseSqlServer(@"Server=.;Database=Shop;Trusted_Connection=True;")
                .Options;
            return new ShopContext(options);
        }
    }
}