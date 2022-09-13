
using System.ComponentModel.DataAnnotations;


namespace CoffeeShop.Models
{
    public class Drink
    {

        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string img { get; set; }
        public string price { get; set; }

        public bool isAlcohol { get; set; }

        public int amount { get; set; }
        
        [Required]
        public int popular { get; set; }

        public bool isBusiness { get; set; }

        public Drink()
        {
            popular = 0;
            
        }
        public Drink(string name, string img,string price, bool al, int am, bool business = false)
        {
            popular = 0;
            this.price = price;
            this.name = name;
            this.img = img;
            isAlcohol = al;
            amount = am;
            isBusiness = business;
        }
    }
}