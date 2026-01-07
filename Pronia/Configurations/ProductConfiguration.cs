using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x=>x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x=>x.Description).IsRequired(false).HasMaxLength(256);

        builder.Property(x => x.Price).HasPrecision(10, 2).IsRequired();

        builder.ToTable(x => x.HasCheckConstraint("CK_Product_Price", "[Price]>=0"));

        builder.Property(x => x.CategoryId).IsRequired();

        builder.HasOne(x => x.Category).WithMany(x=>x.Products).HasForeignKey(x=>x.CategoryId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ProductImages).WithOne(x => x.Product).HasForeignKey(x =>x.ProductID).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ProductTags).WithOne(x => x.Product).HasForeignKey(x=>x.ProductId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);


    
    }
}


