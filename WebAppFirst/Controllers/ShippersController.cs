using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppFirst.Models;

namespace WebAppFirst.Controllers
{
    public class ShippersController : Controller
    {
        // GET: Shippers
            NorthwindEntities db = new NorthwindEntities(); //Uusi ilmentymä tietokannasta nimellä db
        public ActionResult Index()
        {
            //List<Shippers> model = db.Shippers.ToList();    //Lista nimeltä model, joka ottaa vastaan Products kategoriaan kuuluvia rivejä, jotka sijoitetaan db oliosta listana.
            ////db.Dispose();                                   //Suljetaan tietokanta yhteys

            //return View(model);                             //Palautetaan näkymälle lista model.
            var shippers = db.Shippers.Include(s => s.Region); //Ikäänkuin Join kahden taulun välillä.
            return View(shippers.ToList());
        }
        [HttpGet]
        public ActionResult Edit(int? id) //parametri tutkitaan ja etsitään taulusta vastaavaa riviä. Jos vastaavuus löytyy, Näytetään Edit.cshtml sivu.
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Shippers shippers = db.Shippers.Find(id);
            if (shippers == null) return HttpNotFound(); //Jos vastaavuutta ei löydy, palautetaan sivua ei löydy virhe.
            ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription",shippers.RegionID);
            return View(shippers);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Katso https://go.microsoft.com/fwlink/?LinkId=317598
        public ActionResult Edit([Bind(Include = "ShipperID,CompanyName,Phone,RegionID")] Shippers shipper) //Tämä metodi tallentaa muutokset kantaan ja paulauttaa sivun Shippers.cshtml
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipper).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription",shipper.RegionID);
                return RedirectToAction("Index");
            }
            return View(shipper);
        }
        public ActionResult Create() //Tätä metodia kutsutaan listanäkymästä ja tämä metodi näyttää luontinäytön Create.cshtml
        {
            ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShipperID,CompanyName,Phone,RegionID")] Shippers shipper) //Tätä metodia kutsutaan luontinäytöltä, kun klikataan "Save".Metodi lisää uuden rivin tiedot kantaan.
        {
            if (ModelState.IsValid)
            {
                db.Shippers.Add(shipper);

                db.SaveChanges();
                ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription", shipper.RegionID);

                return RedirectToAction("Index");
            }
            return View(shipper);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Shippers shippers = db.Shippers.Find(id);
            if (shippers == null) return HttpNotFound();
            return View(shippers);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shippers shippers = db.Shippers.Find(id);
            db.Shippers.Remove(shippers);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int? id) //parametri tutkitaan ja etsitään taulusta vastaavaa riviä. Jos vastaavuus löytyy, Näytetään Edit.cshtml sivu.
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Shippers shippers = db.Shippers.Find(id);
            if (shippers == null) return HttpNotFound(); //Jos vastaavuutta ei löydy, palautetaan sivua ei löydy virhe.
            return View(shippers);
        }
    }
}