using CoffeeShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CoffeeShop.Dal
{
    public class BusinessDal : DbContext
    {
        public DbSet<Business> Business { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Business>().ToTable("Business");
        }
    }
}



/*
id 1
did 4   esspresso      search -> dronkDb.drinks.find(this.did)
aid 9   pina colada     search _> dronkDb.drinks.find(this.did)
old price = esspresso +  pina colada
new price = old price - 10%

 */