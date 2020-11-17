using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppFirst.Models;
using PagedList;
using WebAppFirst.ViewModels;
using System.Runtime.Remoting.Messaging;

namespace WebAppFirst.Controllers
{
    public class OrdersController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();
        //Adding one test line to test the functionaliti of the gitignore file that was added.
        //One more line for testing purposes.
        // GET: Orders
        // TODO: Lisää toiminnallisuudet HOKS!!! Haettava tieto on Shippers taulussa ja Orders tauluissa Company Name! ShipName hämää.....
        public ActionResult OrderSummary()
        {
            var orderSummary = from o in db.Orders
                               join od in db.Order_Details on o.OrderID equals od.OrderID
                               join p in db.Products on od.ProductID equals p.ProductID
                               join c in db.Categories on p.CategoryID equals c.CategoryID
                               //where lause
                               //orderby lause
                               select new OrderSummaryData
                               {
                                   OrderID = (int)o.OrderID,
                                   CustomerID = o.CustomerID,
                                   EmployeeID = (int)o.EmployeeID,
                                   OrderDate = (DateTime)o.OrderDate,
                                   RequiredDate = (DateTime)o.RequiredDate,
                                   ShippedDate = (DateTime)o.ShippedDate,
                                   ShipVia = (int)o.ShipVia,
                                   Freight = (float)o.Freight,
                                   ShipName = o.ShipName,
                                   ShipAddress = o.ShipAddress,
                                   ShipCity = o.ShipCity,
                                   ShipRegion = o.ShipRegion,
                                   ShipPostalCode = o.ShipPostalCode,
                                   ShipCountry = o.ShipCountry,
                                   ProductID = p.ProductID,
                                   UnitPrice = (int)p.UnitPrice,
                                   Quantity = (int)od.Quantity,
                                   Discount = (float)od.Discount,
                                   ProductName = p.ProductName,
                                   SupplierID = (int)p.SupplierID,
                                   CategoryID = (int)c.CategoryID,
                                   QuantityPerUnit = p.QuantityPerUnit,
                                   UnitsInStock = (int)p.UnitsInStock,
                                   UnitsOnOrder = (int)p.UnitsOnOrder,
                                   ReorderLevel = (int)p.ReorderLevel,
                                   Discontinued = p.Discontinued,
                                   ImageLink = p.ImageLink,
                                   CategoryName = c.CategoryName,
                                   Description = c.Description,
                                   //Picture = (Image)c.Picture


                                   

                               };
            return View(orderSummary);
        }

        public ActionResult TilausOtsikot()
        {
            var orders = db.Orders.Include(o => o.Customers).Include(o => o.Employees).Include(o => o.Shippers);
            return View(orders.ToList()); 
        }

        public ActionResult Index(string sortOrder,string searchString1, string currentFilter1, int? page, int? pagesize,string ShipperCategory, string currentShipperCategory)
        {   //Lajittelu
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CompanyNameSortParm = String.IsNullOrEmpty(sortOrder) ? "companyname_desc" : "";
            ViewBag.OrderDateSortParm = sortOrder == "OrderDate" ? "orderdate_desc" : "OrderDate";
            //Sivutus
            if (searchString1 != null)
            {
                page = 1;
            }
            else
            {
                searchString1 = currentFilter1;
            }

            ViewBag.currentFilter1 = searchString1;
            //Kategoriahaku muistiin
            if ((ShipperCategory != null) && (ShipperCategory != "0"))
            {
                page = 1;
            }
            else
            {
                ShipperCategory = currentShipperCategory;
            }

            ViewBag.currentShipperCategory = ShipperCategory;



            //Dropdownbox
            List<Shippers> lstShippers = new List<Shippers>();

            var shipperList = from cat in db.Shippers
                              select cat;

            Shippers tyhjaShippers = new Shippers();
            tyhjaShippers.ShipperID = 0;
            tyhjaShippers.CompanyName = "";
            tyhjaShippers.ShipperIDShipperName = "";
            lstShippers.Add(tyhjaShippers);

            foreach ( Shippers shipper in shipperList)
            {
                Shippers yksiShipper = new Shippers();
                yksiShipper.ShipperID = shipper.ShipperID;
                yksiShipper.CompanyName = shipper.CompanyName;
                yksiShipper.ShipperIDShipperName = shipper.ShipperID.ToString() + " - " + shipper.CompanyName;
                lstShippers.Add(yksiShipper);
            }
            ViewBag.ShipperID = new SelectList(lstShippers, "ShipperID", "ShipperIDShipperName", ShipperCategory);

            //Dropdownboxjatkuu...Tarvitaanko???
            //NorthwindEntities db = new NorthwindEntities();
            var rahtarit = from s in db.Shippers
                         select s;
            if (!String.IsNullOrEmpty(searchString1))
            {
                rahtarit = rahtarit.Where(s => s.CompanyName.Contains(searchString1));
            }
            if (!String.IsNullOrEmpty(ShipperCategory) && (ShipperCategory != "0")) 
            {
                int para = int.Parse(ShipperCategory);
                rahtarit = rahtarit.Where(p => p.ShipperID == para);
            }


            //Lajittelu
            var orders = from o in db.Orders.Include(o => o.Customers).Include(o => o.Employees).Include(o => o.Shippers)
                         select o;
            if (!String.IsNullOrEmpty(searchString1))
            {
                switch (sortOrder)
                {
                    case "companyname_desc":
                        orders = orders.Where(s =>s.Shippers.CompanyName.Contains(searchString1)).OrderByDescending(s => s.Shippers.CompanyName);
                        break;
                    case "OrderDate":
                        orders = orders.Where(s => s.Shippers.CompanyName.Contains(searchString1)).OrderBy(o => o.OrderDate); //Voi vaatia muutoksen s:ksi.
                        break;
                    case "OrderDate_desc":
                        orders = orders.Where(s => s.Shippers.CompanyName.Contains(searchString1)).OrderByDescending(o => o.OrderDate);
                        break;
                    default:
                        orders = orders.Where(s => s.Shippers.CompanyName.Contains(searchString1)).OrderBy(s => s.Shippers.CompanyName);
                        break;

                }
                orders = orders.Where(s => s.Shippers.CompanyName.Contains(searchString1));
            }
            else if (!String.IsNullOrEmpty(ShipperCategory) && (ShipperCategory != "0"))//Jos käytössä rajaus käytetään sitä ja lajitellaan.
            {
                int para = int.Parse(ShipperCategory);
                switch (sortOrder)
                {
                    case "companyname_desc":
                        orders = orders.Where(s => s.Shippers.ShipperID == para).OrderByDescending(s => s.Shippers.CompanyName);
                        break;
                    case "OrderDate":
                        orders = orders.Where(s => s.Shippers.ShipperID == para).OrderBy(o => o.OrderDate); //Voi vaatia muutoksen s:ksi.
                        break;
                    case "OrderDate_desc":
                        orders = orders.Where(s => s.Shippers.ShipperID == para).OrderByDescending(o => o.OrderDate);
                        break;
                    default:
                        orders = orders.Where(s => s.Shippers.ShipperID == para).OrderBy(s => s.Shippers.CompanyName);
                        break;

                }
            }
            else //Ei hakuehtoa
            {
                switch (sortOrder)
                {
                    case "companyname_desc":
                        orders = orders.OrderByDescending(s => s.Shippers.CompanyName);
                        break;
                    case "OrderDate":
                        orders = orders.OrderBy(o => o.OrderDate); //Voi vaatia muutoksen s:ksi.
                        break;
                    case "OrderDate_desc":
                        orders = orders.OrderByDescending(o => o.OrderDate);
                        break;
                    default:
                        orders = orders.OrderBy(s => s.Shippers.CompanyName);
                        break;
                
                }

            }

            //Sivutus
            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);

            return View(orders.ToPagedList(pageNumber,pageSize));
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", orders.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", orders.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", orders.ShipVia);
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", orders.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", orders.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", orders.ShipVia);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", orders.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", orders.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", orders.ShipVia);
            return View(orders);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            db.Orders.Remove(orders);
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
