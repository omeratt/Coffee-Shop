using CoffeeShop.Dal;
using CoffeeShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Controllers
{
    public class BaristaController : Controller
    {
        private OrdersDal orders = new OrdersDal();
        private drinksDal drinks = new drinksDal();
        private UserDal us = new UserDal();

        // GET: Barista
        public ActionResult Index()
        {

            return View(orders.orders.ToList<Order>());
        }

        public ActionResult SearchOrders()
        {
            List<Order> ord = orders.orders.ToList<Order>();
            List<string> Names = null;
            String searchedValue = Request.Form["searchInput"];
            if(searchedValue.Equals(""))
                return PartialView("search", ord);
            bool z = us.Users.AsEnumerable().Where(u => u.name.Contains(searchedValue)).ToList().Count > 0;
            if (z)
            {
                Names = us.Users.Where(u => u.name.ToLower().Contains(searchedValue.ToLower())).Select(u => u.uid.ToString()).ToList();

                
            }
            
            if(Names != null)
                ord = (from x in orders.orders where Names.Any(s => s.ToLower().Contains(x.uid.ToLower())) select x).ToList<Order>();
            else
                ord = (from x in orders.orders where x.uid.ToLower().Contains(searchedValue.ToLower()) select x).ToList<Order>();


            return PartialView("search", ord);
        }

        public ActionResult Confirm()
        {
           
            string oid = Request.Form["oid"];
            string strcon = ConfigurationManager.ConnectionStrings["OrdersDal"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();           
            SqlCommand cmd = new SqlCommand(" UPDATE Orders  SET confirm = 1 Where id = " + oid + "", con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                //updated success                 
            }
            List<Order> ord = orders.orders.ToList<Order>();
            return PartialView("search",ord);
        }

        public List<Drink> OrdersPerOrder(Order o)
        {
            List<Drink> l = new List<Drink>();

            return l;
        }

        [HttpPost]
        public ActionResult UpdateOrder()
        {
            string oid = Request.Form["oid"];
            string did = Request.Form["did"];
            string quant = Request.Form["quantity"];
            float price = calcTotalPrice(int.Parse(oid), int.Parse(did),int.Parse(quant));

            string strcon = ConfigurationManager.ConnectionStrings["OrdersDal"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand(" UPDATE Orders  SET amount = " + quant + ", price = '" + price + "'  Where id = " + oid + " And did = " + did + "", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Close();
            cmd = new SqlCommand(" UPDATE Orders  SET price = '" + price + "'  Where id = " + oid + "", con);
            dr = cmd.ExecuteReader();
            //ViewBag.str = oid;
            List<Order> ord = orders.orders.ToList<Order>();
           //ystem.Threading.Thread.Sleep(1000);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteOrder(int dx, int dy)
        {
            UpdateOrder(dx, dy);
            orders.orders.Remove(orders.orders.Find(dx,dy));
            orders.SaveChanges();
            return RedirectToAction("Index");
        }

        public float calcTotalPrice(int oid, int did,int quant)
        {

            Order ord = orders.orders.Find(oid, did);
            Drink d = drinks.Drink.Find(did);
            int prevQuant = ord.amount;
            float total = float.Parse(ord.price);
            if(quant > prevQuant)
            {
                total = float.Parse(ord.price) + float.Parse(d.price) * (quant-prevQuant);
            }
            else if(quant < prevQuant)
            {
                total = float.Parse(ord.price) + float.Parse(d.price) * (quant - prevQuant);
            }

            return total;

        }

        public void UpdateOrder(int id, int did)
        {

            Order ord = orders.orders.Find(id, did);
            float price = float.Parse(ord.price) - float.Parse(drinks.Drink.Find(did).price) * ord.amount;
            string strcon = ConfigurationManager.ConnectionStrings["OrdersDal"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand(" UPDATE Orders  SET price = '" + price + "'  Where id = " + id + "", con);
            SqlDataReader dr = cmd.ExecuteReader();
            
        }


    }
}