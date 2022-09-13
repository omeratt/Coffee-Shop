using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Dal;
using CoffeeShop.Models;

namespace CoffeeShop.Controllers
{
    public class TableOrdersController : Controller
    {
        private TableOrderDal db = new TableOrderDal();

        // GET: TableOrders
        public ActionResult Index()
        {
            return View(db.TableOrder.ToList());
        }

        // GET: TableOrders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableOrder tableOrder = db.TableOrder.Find(id);
            if (tableOrder == null)
            {
                return HttpNotFound();
            }
            return View(tableOrder);
        }

        // GET: TableOrders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TableOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Date,Uid,Tid,NumberOfSeats")] TableOrder tableOrder)
        {
            if (ModelState.IsValid)
            {
                db.TableOrder.Add(tableOrder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tableOrder);
        }

        // GET: TableOrders/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableOrder tableOrder = db.TableOrder.Find(id);
            if (tableOrder == null)
            {
                return HttpNotFound();
            }
            return View(tableOrder);
        }

        // POST: TableOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Date,Uid,Tid,NumberOfSeats")] TableOrder tableOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tableOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tableOrder);
        }

        // GET: TableOrders/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableOrder tableOrder = db.TableOrder.Find(id);
            if (tableOrder == null)
            {
                return HttpNotFound();
            }
            return View(tableOrder);
        }

        // POST: TableOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TableOrder tableOrder = db.TableOrder.Find(id);
            db.TableOrder.Remove(tableOrder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
