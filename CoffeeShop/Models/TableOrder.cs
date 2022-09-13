using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShop.Models
{
    public class TableOrder
    {
        [Key]
        [Column(Order = 0)]
        public String Date { get; set; } /*DateTime.Now.ToString("dd/MM/yyyy HH:mm")	29/05/2015 05:50*/

        public string Uid { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Tid { get; set; }

        public int NumberOfSeats { get; set; }

        public TableOrder(string date, string uid, int tid, int numberOfSeats)
        {
            Date = date;

            Uid = uid;

            Tid = tid;

            NumberOfSeats = numberOfSeats;
        }

        public TableOrder() { }

    }
}