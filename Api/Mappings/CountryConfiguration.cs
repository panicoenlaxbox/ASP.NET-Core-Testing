using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Mappings
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");
            builder.Property(c => c.Name).IsRequired();
            // Data seeding
            // During migration, ef will activate IDENTITY_INSERT

            //builder.HasData(
            //    new Country() { Id = 1, Name = "Spain" },
            //    new Country() { Id = 2, Name = "France" }
            //);
        }
    }
}