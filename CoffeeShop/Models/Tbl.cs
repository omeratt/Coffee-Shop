
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class Tbl
    {
        [Key]
        public int tid { get; set; }

        public int amount { get; set; }

        public bool isIn { get; set; }

    }
}