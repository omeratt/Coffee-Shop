using CoffeeShop.Dal;
using CoffeeShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace CoffeeShop.Controllers
{
    public class HomeController : Controller
    {

        private drinksDal db = new drinksDal();
        private TblDal tableDB = new TblDal();
        private UserDal userDB = new UserDal();
        private TableOrderDal tableOrderDB = new TableOrderDal();
        private OrdersDal orders = new OrdersDal();
        private Dictionary<Drink, int> cartDictionary = new Dictionary<Drink, int>();
        // GET: Account
        public ActionResult Index()
        {
            ViewBag.drinks = db.Drink.ToList();            
            return View();
        }

        public ActionResult SearchItems()
        {
            String searchedValue = Request.Form["searchInput"];
            List<Drink> drinks = db.Drink.ToList<Drink>();
            drinks = (from x in db.Drink where x.name.Contains(searchedValue) select x).ToList<Drink>();
            TempData["SearchItems"] = drinks;
            return RedirectToAction("Menu");

        }

        public ActionResult Menu(string sort)
        {
            ViewBag.sort = db.Drink.ToList<Drink>();

            string s = Request.Form["sortSelect"];

            if (TempData["SearchItems"] != null)
                ViewBag.sort = TempData["SearchItems"];
            else if (s == null)
                return View();
            else if (s.Equals("in-stock"))
                ViewBag.sort = DescByAmount();
            else if (s.Equals("name-asc"))
                ViewBag.sort = AscByName();
            else if (s.Equals("name-desc"))
                ViewBag.sort = DescByName();
            else if (s.Equals("least-popular"))
                ViewBag.sort = AscByPop();
            else if (s.Equals("most-popular"))
                ViewBag.sort = DescByPop();
            else if (s.Equals("price-asc"))
                ViewBag.sort = AscByPrice();
            else if (s.Equals("price-desc"))
                ViewBag.sort = DescByPrice();
            else if (s.Equals("Business lunch"))
                ViewBag.sort = sortByBusinessLunch();

            return View();
        }

        public ActionResult CheckOut()
        {
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];
            float disc = calcDiscount(dict);
            float total = calcTotal(dict);
            Session["take"] = Request.Form["take"];
            List<float> lst = new List<float>
            {
                total,disc
            };
            
            return View("Checkout",lst);
        }

        public ActionResult Pay()
        {
            int index = orders.orders.Count();
            int id = 0 ,tid=-1;
            if (index > 0)
                id = orders.orders.ToList<Order>()[index-1].id + 1;
            bool confirm = false, take;
            string tdate = null, date = DateTime.Now.ToString();

            Tuple<string, int> tableOrderKey;
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];

            float totalPrice = calcTotal(dict) - calcDiscount(dict);

            if ((Session["take"] != null && Session["take"].ToString().Contains("on")) || (Session["isBookedTable"] != null && !bool.Parse(Session["isBookedTable"].ToString())))
            {
                take = true;
                if (Session["isBookedTable"] != null && bool.Parse(Session["isBookedTable"].ToString()) == true)
                {
                    RemoveTableOrder((Tuple<string, int>)Session["orderId"]);
                    Session["isBookedTable"] = false;
                }
            }
            else
                take = false;

            //check if user connected, if not its a guest and have a name field in form
            string name = "";
            if (Session["email"] == null)
                name = "Guest " + Request.Form["fn"] +" "+ Request.Form["ln"];
            else
            {               
                name = Session["Uid"].ToString();
            }

            if (Session["orderId"] != null && take == false) {
                tableOrderKey = (Tuple<string, int>)Session["orderId"];
                TableOrder tblOrder = tableOrderDB.TableOrder.Find(tableOrderKey.Item1, tableOrderKey.Item2);
                tdate = tblOrder.Date;
                tid = tblOrder.Tid;
            }
            foreach (Drink d in dict.Keys)
            {
                orders.orders.Add(new Order(id, name, d.id, dict[d], confirm, tid, tdate, date, take , totalPrice.ToString()));
                orders.SaveChanges();
            }

            //client pay his order->reset cart dictionary session and declare flag for not changing table.
            //client buy and take away -> he want to buy again -> he can order table
            //if client already paid and not take away-> he ordered table -> cant change his table
            dict.Clear();
            Session["isPay"] = true;
            ViewBag.drinks = db.Drink.ToList();
            TempData["msg"] = "payment";
            return RedirectToAction("Index");
        }

        public ActionResult RemoveItemFromCart(int? did)
        {
            int id = int.Parse(did.ToString());
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];
            Drink d = dict.Keys.ToList().Where(dr => (dr.id == id)).ToList()[0];
            dict.Remove(d);
            return RedirectToAction("Cart");
        }

        public ActionResult AddToCart()
        {
            
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];

            int p = int.Parse(Request.Form["prod"]);
            Drink d = db.Drink.Find(p);
            
            if((ContainDrink(d, dict))!=null && dict[(ContainDrink(d,dict))] == d.amount)
            {
                return PartialView("ProdCount", -1); //
            }


            Session["CartCount"] = int.Parse(Session["CartCount"].ToString()) + 1;
            ((List<Drink>)Session["CartProd"]).Add(d);
            int count = ((List<Drink>)Session["CartProd"]).Count;


            addDrinkToDictionary(d,dict);
            ViewBag.Discount = calcDiscount(dict);
            return PartialView("ProdCount",dict.Count);
        }

        public ActionResult Cart() {
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];
            //check if there is an alcohol in cart for age validation
            if (dict.Keys.Where(d => d.isAlcohol).Count() > 0)
                Session["AgeLimit"] = true;
            else
                Session["AgeLimit"] = false;

            ViewBag.Discount = calcDiscount(dict);
            ViewBag.Total = calcTotal(dict);
            return View();

        }

        public ActionResult totalDrink(Drink d)
        {
            return PartialView("new1",d);
        }
        public ActionResult UpdateCart()
        {
            
            int newAmount = int.Parse(Request.Form["quantity"]);
            int did = int.Parse(Request.Form["did"]);
            //int newAmount = int.Parse(TempData["quantity"].ToString());
            //Drink d = (Drink)TempData["change"];
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];
            Drink d = dict.Keys.ToList().Where(dr => (dr.id == did)).ToList()[0];
            //int newAmount = int.Parse(qua);
            if (dict[d] < newAmount)
            {
                dict[d] += 1;
            }
            else
            {
                dict[d] -= 1;
            }
            float disc = calcDiscount(dict);
            float total = calcTotal(dict);

            List<float> lst = new List<float>
            {
                total,disc
            };
            TempData["disc"] = disc;
            TempData["total"] = total;
            return PartialView("new",lst);
        }

        public ActionResult UpdateCart1()
        {
            List<string> sa = Request.Form.AllKeys.ToList();
            string newAmounts = Request.Form["quantity"];
            Dictionary<Drink, int> dict = HandleChangesInCart(newAmounts);
            return RedirectToAction("Cart");
        }


        /*    Cart Help Functions      */
        public bool isContainDrink(Dictionary<Drink, int> dict, Drink drink)
        {
            foreach(Drink d in dict.Keys)
            {
                if (d.id == drink.id)
                {
                    dict[d]++;
                    return true;
                }
            }
            return false;
        }

        public Drink ContainDrink(Drink drink, Dictionary<Drink, int> dict)
        {
            if (dict.Count() <= 0)
                return null;
            
            foreach (Drink d in dict.Keys)
            {
                if (d.id == drink.id)
                {
                    return d;
                }
            }
            return null;
        }

        public void addDrinkToDictionary(Drink d, Dictionary<Drink, int> dict)
        {
            if (!isContainDrink(dict, d))
                dict.Add(d, 1);

        }

        public float calcDiscount(Dictionary<Drink, int> dict)
        {
            //check if there is a business launch in cart
            float discount = calcBusinessDiscount(dict);
            if (!isVip())
                return discount;

            int count = 0;
            //check if he is vip and give him discount every 10 coffees
            foreach (Drink drink in dict.Keys)
            {
                if (drink.isAlcohol)
                    continue;

                
                count += dict[drink];
                if((count/10) > 0)
                    discount += (count/10) * float.Parse(drink.price); 
            }
            
            return discount;
        }

        public float calcBusinessDiscount(Dictionary<Drink, int> dict)
        {
            float disc = 0;
            List<Drink> businessLst = dict.Keys.ToList().Where(d => d.isBusiness).ToList();
            if (businessLst.Count == 0)
                return disc;

            foreach(Drink drink in businessLst)
            {
                disc += (float.Parse(drink.price) * (float)0.1) * dict[drink];
            }

            return disc;

        }


        public float calcTotal(Dictionary<Drink, int> dict)
        {
            float total = 0;
            foreach(Drink d in dict.Keys)
            {
                total += float.Parse(d.price) * dict[d];
            }

            return total;
        }

        public bool isVip()
        {    
            if (Session["email"] == null)
                return false;

            return userDB.Users.Find(int.Parse(Session["Uid"].ToString())).isVip;
        }

        public void DeleteFromCart()
        {

        }

        public Dictionary<Drink, int> HandleChangesInCart(string s)
        {
            Dictionary<Drink, int> dict = (Dictionary<Drink, int>)Session["CartDict"];
            string[] strLst;
            if (!s.Contains(","))
                return dict;

            strLst = s.Split(',');
            int count = 0;
            foreach(Drink d in dict.Keys.ToList())
            {
                int newAmount = int.Parse(strLst[count++]);

                if (newAmount == 0)
                    dict.Remove(d);
                else if (newAmount == 1)
                    continue;
                else
                dict[d] = newAmount;
            }

            return dict;
        }


        /*                             */

        /***********************sorting menu helper functions************************/
        public List<Drink> AscByAmount()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.OrderBy(pop => pop.amount).ToList<Drink>();
            return d;

        }
        public List<Drink> DescByAmount()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.OrderByDescending(pop => pop.amount).ToList<Drink>();
            return d;

        }
        public List<Drink> AscByName()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.OrderBy(pop => pop.name).ToList<Drink>();
            return d;

        }
        public List<Drink> DescByName()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.OrderByDescending(pop => pop.name).ToList<Drink>();
            return d;

        }
        public List<Drink> AscByPop()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.AsEnumerable().OrderBy(pop => pop.popular).ToList();
            return d;

        }
        public List<Drink> DescByPop()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.OrderByDescending(pop => pop.price).ToList();
            return d;

        }
        public List<Drink> AscByPrice()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.AsEnumerable().OrderBy(pop => float.Parse(pop.price)).ToList();
            return d;

        }
        public List<Drink> DescByPrice()
        {

            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.AsEnumerable().OrderByDescending(pop => float.Parse(pop.price)).ToList();
            return d;

        }

        public List<Drink> sortByBusinessLunch()
        {
            List<Drink> d = db.Drink.ToList<Drink>();
            d = db.Drink.AsEnumerable().Where(drink=>drink.isBusiness).ToList();
            return d;
        }
        /**************************************************************************/

        /***********************booking table and  helper functions************************/
        [HttpPost]
        public ActionResult BookTable()
        {
            string dateRequest = Request.Form["date"];
            string time = Request.Form["time"];
            string dateParsedToString = fixDate(dateRequest) + ' ' + time;
            string date = DateTime.Parse(dateParsedToString).ToString("dd/MM/yyyy HH:mm");
            string inOut = Request.Form["insideOutside"];
            bool isIn = false;
            if (inOut.Equals("Inside"))
                isIn = true;

            string numberOfSeats = Request.Form["numberOfSeats"];
            CheckAvailableAndBookOrder(numberOfSeats, isIn, date);

            return RedirectToAction("Index");
        }

        public string fixDate(string date)
        {
            string[] seperateDate = date.Split('/');
            if (seperateDate[0].Count() == 1)
                seperateDate[0] = '0' + seperateDate[0];
            if (seperateDate[1].Count() == 1)
                seperateDate[1] = '0' + seperateDate[1];

            //swap mm/dd to dd/mm
            string temp = seperateDate[0];
            seperateDate[0] = seperateDate[1];
            seperateDate[1] = temp;

            return String.Join("/", seperateDate);
        }

        public void BookOrderTable(string date, int tid, string numberOfSeats)
        {
            //check if user connected, if not its a guest and have a name field in form
            string name = "";
            if (Session["email"] == null)
                name = "Guest " + Request.Form["Name"];
            else
            {
                user us = userDB.Users.Find(int.Parse(Session["Uid"].ToString()));
                name = us.role + " " + us.name;
            }


            TableOrder tableOrder = new TableOrder(date, name, tid, int.Parse(numberOfSeats));
            tableOrderDB.TableOrder.Add(tableOrder);
            tableOrderDB.SaveChanges();
        }

        public bool CheckIfAvailable(string date, int tid)
        {
            string strcon = ConfigurationManager.ConnectionStrings["TableOrderDal"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand("select * from TableOrder where date='" + date + "' AND tid='" + tid + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                Response.Write("<script>alert('Table is not available')</script>");
                return false;
            }
            return true;

        }

        public bool isOutsideTableDays(string date)
        {
            return !(DateTime.Parse(date).DayOfWeek.ToString().Equals("Tuesday") || DateTime.Parse(date).DayOfWeek.ToString().Equals("Friday"));
        }
        public void CheckAvailableAndBookOrder(string numberOfSeats, bool isIn, string date, string name = null)
        {
            //Outside tables not available in Tusday or Friday 
            if(!isIn && !isOutsideTableDays(date))
            {
                while (true)
                {
                    Response.Write("<script>alert('Outside are not available in Tusday or Friday')</script>");                    
                    return;
                }
            }
            
            List<Tbl> tableList = tableDB.tbls.AsEnumerable().Where(tb => (tb.amount >= int.Parse(numberOfSeats))).ToList();
            if (tableList.Count() == 0)
                return;

            //check if there is table inside with this numer of seats
            List<Tbl> tl = tableList.AsEnumerable().Where(tb => (tb.isIn == isIn)).ToList();
            if (tl.Count() == 0)
                return;

            //check if table already taken
            foreach (Tbl t in tl)
            {
                if (CheckIfAvailable(date, t.tid))
                {
                    BookOrderTable(date, t.tid, numberOfSeats);
                    Session["isBookedTable"] = true;
                    Session["orderId"] = new Tuple<string, int>(date, t.tid);
                    return;
                }
            }
        }

        public ActionResult BookTableCart()
        {
            string isUpdate = Request.Form["submitBtn"];
            if (Session["orderId"] != null && isUpdate.Equals("Update"))
            {
                RemoveTableOrder((Tuple<string, int>)Session["orderId"]);
            }

            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string inOut = Request.Form["insideOutside"];
            bool isIn = false;
            if (inOut.Equals("Inside"))
                isIn = true;
            string numberOfSeats = Request.Form["numberOfSeats"];

            CheckAvailableAndBookOrder(numberOfSeats, isIn, date);
            return RedirectToAction("Cart");
        }


        public void RemoveTableOrder(Tuple<string, int> tup)
        {
            tableOrderDB.TableOrder.Remove(tableOrderDB.TableOrder.Find(tup.Item1, tup.Item2));
        }
        /**************************************************************************/
    }
}
