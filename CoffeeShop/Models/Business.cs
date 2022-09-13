
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models
{
    public class Business
    {
        [Key]
        public int id { get; set; }
        public int did { get; set; }
        public int aid { get; set; }

        public string oldprice { get; set; }

        public string newprice { get; set; }

        public Business(int id, int did, int aid)
        {
            this.id = id;
            this.did = did;
            this.aid = aid;
            setPrices();
        }

        public void setPrices()
        {
            Dal.drinksDal drinkDb = new Dal.drinksDal();
            float price = float.Parse(drinkDb.Drink.Find(did).price) + float.Parse(drinkDb.Drink.Find(aid).price);
            double newPrice = price - (0.1 * price);
            this.oldprice = price.ToString();
            this.newprice = newPrice.ToString();
        }
    }
}