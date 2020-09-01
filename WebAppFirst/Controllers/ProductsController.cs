using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppFirst.Models;

namespace WebAppFirst.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            NorthwindEntities db = new NorthwindEntities(); //Uusi ilmentymä tietokannasta nimellä db
            List<Products> model = db.Products.ToList();    //Lista nimeltä model, joka ottaa vastaan Products kategoriaan kuuluvia rivejä, jotka sijoitetaan db oliosta listana.
            db.Dispose();                                   //Suljetaan tietokanta yhteys

            return View(model);                             //Palautetaan näkymälle lista model.
        }
    }
}