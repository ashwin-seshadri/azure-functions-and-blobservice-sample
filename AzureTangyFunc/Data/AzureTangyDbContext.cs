using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureTangyFunc.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureTangyFunc.Data
{
    public class AzureTangyDbContext : DbContext
    {
        public AzureTangyDbContext(DbContextOptions<AzureTangyDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<SalesRequest> SalesRequests { get; set; }
        public DbSet<GroceryItem> GroceryItems { get; set; }    


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SalesRequest>().HasKey(x => x.Id);
            modelBuilder.Entity<GroceryItem>().HasKey(x => x.Id);
        }
    }
}
