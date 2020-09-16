using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppFirst.Models;

namespace WebAppFirst.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
            NorthwindEntities db = new NorthwindEntities(); //Uusi ilmentymä tietokannasta nimellä db
        public ActionResult Index()
        {
            List<Products> model = db.Products.ToList();    //Lista nimeltä model, joka ottaa vastaan Products kategoriaan kuuluvia rivejä, jotka sijoitetaan db oliosta listana.
            db.Dispose();                                   //Suljetaan tietokanta yhteys

            return View(model);                             //Palautetaan näkymälle lista model.
        }

        public ActionResult Index2()
        {
            NorthwindEntities db = new NorthwindEntities(); //Uusi ilmentymä tietokannasta nimellä db
            List<Products> model = db.Products.ToList();    //Lista nimeltä model, joka ottaa vastaan Products kategoriaan kuuluvia rivejä, jotka sijoitetaan db oliosta listana.
            db.Dispose();                                   //Suljetaan tietokanta yhteys

            return View(model);                             //Palautetaan näkymälle lista model.
        }
        public ActionResult Details(int? id) //parametri tutkitaan ja etsitään taulusta vastaavaa riviä. Jos vastaavuus löytyy, Näytetään Edit.cshtml sivu.
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("login", "home");
            }
            else
            {
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                Products tuote = db.Products.Find(id);
                if (tuote == null) return HttpNotFound(); //Jos vastaavuutta ei löydy, palautetaan sivua ei löydy virhe.
                return View(tuote);
            }
        }
    }
}