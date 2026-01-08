using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pronia.Models;
using System.Reflection;

namespace Pronia.Contexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
            base.OnConfiguring(optionsBuilder);
        }*/

        public AppDbContext(DbContextOptions option):base(option)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
        

        public DbSet<Card> Cards { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

    }
}
