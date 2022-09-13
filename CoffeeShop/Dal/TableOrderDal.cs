using CoffeeShop.Models;
using System.Data.Entity;

namespace CoffeeShop.Dal
{
    public class TableOrderDal : DbContext
    {
        public DbSet<TableOrder> TableOrder { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TableOrder>().ToTable("TableOrder");
        }

    }
}