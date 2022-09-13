using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models
{
    public class Table
    {
        [Key]
        public int tid { get; set; }

        public string amount { get; set; }

        public int taken { get; set; }

        public bool isIn { get; set; }
        
        public List<user> usersList { get; set; }
    }
}