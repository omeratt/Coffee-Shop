using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CoffeeShop.Dal;
using CoffeeShop.Models;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace CoffeeShop.Controllers
{
    public class AdminController : Controller
    {
        private drinksDal db = new drinksDal();
        private UserDal udb = new UserDal();
        private TblDal tdb = new TblDal();
        private TableOrderDal tbldal = new TableOrderDal();
        // GET: Admin
        public ActionResult Index(List<user> users)
        {
            if (TempData["users"] != null)
                users = (List<user>)TempData["users"];
            if (users == null)
                users = udb.Users.ToList<user>();

            List<Drink> drinkList = (db).Drink.ToList<Drink>();

            TempData["users"] = users;
            TempData["drinks"] = drinkList;
            TempData["tables"] = tdb.tbls.ToList();
            TempData["tblorder"] = tbldal.TableOrder.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult SearchUser()
        {
            String searchedValue = Request.Form["searchInput"];
            Console.WriteLine(searchedValue.ToString());
            UserDal userDal = new UserDal();
            List<user> users = (new UserDal()).Users.ToList<user>();
            users = (from x in userDal.Users where x.name.Contains(searchedValue) select x).ToList<user>();

            TempData["users"] = users;
            return PartialView("search",users);

        }


        [HttpPost]
        public ActionResult Create()
        {
            bool al = false;
            bool bus = false;
            string name = Request.Form["name"];
            string img = Request.Form["img"];
            

            if (img.Contains("drink"))
                al = true;
            if (img.Contains("dish") || img.Contains("burger"))
                bus = true;
            string resultString = Regex.Match(img, @"\d+").Value;
            string price = Request.Form["price"];
            int am = int.Parse(Request.Form["amount"]);
            Drink drink = new Drink(name, resultString, price, al, am,bus);
            db.Drink.Add(drink);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult EditCoffee()
        {
            string newPrice = Request.Form["newPrice"];
            string newAmount = Request.Form["newAmount"];

            string coffeeKey = null;
            if (Request.Form.AllKeys.Length != 0)
                coffeeKey = Request.Form.AllKeys[2];

            if (coffeeKey == null)
                return RedirectToAction("Index");


            Drink updatedCoffee = db.Drink.Find(int.Parse(coffeeKey));
            db.Drink.Remove(updatedCoffee);
            db.SaveChanges();

            if (!newPrice.Equals(""))
                updatedCoffee.price = newPrice;
            if (!newAmount.Equals(""))
                updatedCoffee.amount = int.Parse(newAmount);

            db.Drink.Add(updatedCoffee);
            db.SaveChanges();


            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult AddUser()
        {
            UserDal usersd = new UserDal();
            string fn = Request.Form["fn"];
            string email = Request.Form["email"];
            string pass = Request.Form["pass"];
            string role = Request.Form["role"];
            int age = int.Parse(Request.Form["age"]);
            string vip = Request.Form["Vip"];
            bool isVip = false;
            if (vip != null)
                isVip = true;

            if (checkDuplicatesEmails(email))
            {
                Response.Write("<script>alert('Email already exist! Choose another email.')</script>");
                return View("Index");
            }

            user newUser = new user(fn, email, pass, role, age, isVip);
            usersd.Users.Add(newUser);
            usersd.SaveChanges();


            return RedirectToAction("Index", "Admin");
        }


        public ActionResult DeleteDrink()
        {
            string Key = null;
            if (Request.Form.AllKeys.Length != 0)
                Key = Request.Form.AllKeys[0];

            if (Key == null)
                return RedirectToAction("Index");

            Drink updatedCoffee = db.Drink.Find(int.Parse(Key));
            db.Drink.Remove(updatedCoffee);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CreateTable()
        {
            string numberOfSeats = Request.Form["numberOfSeats"];
            string insideOutside = Request.Form["insideOutside"];
            Tbl newTable = new Tbl();
            newTable.amount = int.Parse(numberOfSeats);
            bool isIn = true;
            if (insideOutside.Equals("Outside"))
                isIn = false;

            newTable.isIn = isIn;
            tdb.tbls.Add(newTable);
            tdb.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool checkDuplicatesEmails(string email)
        {
            string strcon = ConfigurationManager.ConnectionStrings["UserDal"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand("select * from users where email='" + email + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
                return true;

            return false;
        }
    }
}