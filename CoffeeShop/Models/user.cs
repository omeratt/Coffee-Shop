
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models
{
    public class user
    {
        public string name { get; set; }
        
        [Key]
        public int uid { get; set; }

        public string email { get; set; }

        public string password { get; set; }
        public string role { get; set; }

        public int coffeBought { get; set; } //how much coffee the users bought

        public int age { get; set; }

        public bool isVip { get; set; }



        public user(string name, string email, string password, string role,int age, bool isVip = false)
        {
            this.name = name;
            this.email = email;
            this.password = password;
            this.role = role;
            this.isVip = isVip;
            if (!isVip && (role.Equals("admin") || role.Equals("barista"))) //the admins and barista must be vips
                this.isVip = true;

            this.age = age;
        }

        public user()
        {
            isVip = false;
        }
    }
}