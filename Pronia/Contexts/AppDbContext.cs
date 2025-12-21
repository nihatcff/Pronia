using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pronia.Models;

namespace Pronia.Contexts
{
    public class AppDbContext : DbContext
    {
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
            base.OnConfiguring(optionsBuilder);
        }*/

        public AppDbContext(DbContextOptions option):base(option)
        {
            
        }

        public DbSet<Card> Cards { get; set; }


    }
}
