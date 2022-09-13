using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models
{
    public class Order
    {
        [Key]
        [Column(Order = 0)]
        public int id { get; set; }

        public string uid { get; set; }

        [Key]
        [Column(Order = 1)]
        public int did { get; set; }
        public int amount { get; set; }
        public bool confirm { get; set; }
        public int tid { get; set; }
        public string tdate { get; set; }
        public string date { get; set; }
        public bool take { get; set; }

        public string price  { get; set; }
        public Order(int id, string uid, int did, int amount, bool confirm, int tid, string tdate, string date, bool take, string price = null)
        {
            this.id = id;
            this.uid = uid;
            this.did = did;
            this.amount = amount;
            this.confirm = confirm;
            this.tid = tid;
            this.tdate = tdate;
            this.date = date;
            this.take = take;
            this.price = price;
        }

        public Order()
        {

        }
    }
}