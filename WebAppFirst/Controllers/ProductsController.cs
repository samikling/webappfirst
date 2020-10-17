using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAppFirst.Models;
using PagedList;
namespace WebAppFirst.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
            NorthwindEntities db = new NorthwindEntities(); //Uusi ilmentymä tietokannasta nimellä db
        public ActionResult Index(string sortOrder,string searchString1,string ProductCategory, string currentFilter1,string currentProductCategory, int? page, int? pagesize)
        {
            //List<Products> tuotteet = db.Products.ToList();    //Lista nimeltä model, joka ottaa vastaan Products kategoriaan kuuluvia rivejä, jotka sijoitetaan db oliosta listana.
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProductNameSortParm = String.IsNullOrEmpty(sortOrder) ? "productname_desc" : "";
            ViewBag.UnitPriceSortParm = sortOrder == "UnitPrice" ? "unitprice_desc" : "UnitPrice";

            if (searchString1 != null)
            {
                page = 1;
            }
            else
            {
                searchString1 = currentFilter1;
            }

            ViewBag.currentFilter1 = searchString1;
            //Tuotecategoriahakufiltterin laitto muistiin
            if ((ProductCategory != null) && (ProductCategory != "0"))
            {
                page = 1;
            }
            else
            {
                ProductCategory = currentProductCategory;
            }

            ViewBag.currenProductCategory = ProductCategory;
            var tuotteet = from p in db.Products
                           select p;

            //Pudotusvalikko
            List<Categories> lstCategories = new List<Categories>();
            //Tuotekategorioiden haku tietokannasta
            var categoryList = from cat in db.Categories
                               select cat;
            //Luetteloon viedään ensin yksi tyhjä rivi
            Categories tyhjaCategory = new Categories();
            tyhjaCategory.CategoryID = 0;
            tyhjaCategory.CategoryName = "";
            tyhjaCategory.CategoryIDCategoryName = "";
            lstCategories.Add(tyhjaCategory);

            //Tietokannasta haetut rivit käsitellään silmukassa ja arvot viedään muuttujiin.
            //Tässä luodaan myös yhdistelmämuuttuja, jossa on sekä avaintieto että sen selitys samassa muuttujassa.
            //Huomaa, että Models-kansion luokkamääritykseen pitää listätä uusi yhdistelmäkenttä.
            foreach (Categories category in categoryList)
            {
                Categories yksiCategory = new Categories();
                yksiCategory.CategoryID = category.CategoryID;
                yksiCategory.CategoryName = category.CategoryName;
                yksiCategory.CategoryIDCategoryName = category.CategoryID.ToString() + " - " + category.CategoryName;
                //Taulun luokkamääritykseen Models-kansiossa piti lisätä tämä "uusi" kenttä = CategoryIDCategoryName
                lstCategories.Add(yksiCategory);
            }
            //Lopuksi luodaan uusi SelectList ja se sijoitetaan ViewBag-olioon(Tätä käytetään View:n puolella pudotusvalikon luettelon muodostuksessa.)
            ViewBag.CategoryID = new SelectList(lstCategories, "CategoryID", "CategoryIDCategoryName", ProductCategory);


            //Tuotteiden haku ja Pagination
            if (!String.IsNullOrEmpty(searchString1))
            {
                tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1));
            }
            //Tuoteryhmällä nolla (Tyhjän valinnan oletusarvo) ei voi hakea, joten se suljetaan pois, jotta tuotehaku kannasta toimisi oikein
            if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0"))
            {
                int para = int.Parse(ProductCategory);
                tuotteet = tuotteet.Where(p => p.CategoryID == para);
            }
            switch (sortOrder)
            {
                case "productname_desc":
                    tuotteet = tuotteet.OrderByDescending(p => p.ProductName);
                    break;
                case "UnitPrice":
                    tuotteet = tuotteet.OrderBy(p => p.UnitPrice);
                    break;
                case "UnitPrice_desc":
                    tuotteet = tuotteet.OrderByDescending(p => p.UnitPrice);
                    break;
                default:
                    tuotteet = tuotteet.OrderBy(p => p.ProductName);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString1)) //Jos hakufiltteri on käytössä, niin käytetään sitä ja sen lisäksi lajitellaan tulokset
            {
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderBy(p => p.ProductName);
                        break;
                }
            }
            else if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0")) //Jos käytössä on tuoteryhmärajaus, niin käytetään sitä ja sen lisäksi lajitellaan.
            {
                int para = int.Parse(ProductCategory);
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderBy(p => p.ProductName);
                        break;

                }
            }
            else
            {
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.OrderBy(p => p.ProductName);
                        break;
                }

            }

            int pageSize = (pagesize ?? 10); //Tämä palauttaa sivukoon taikka jos pagesize on null, niin palauttaa koon 10 riviä per sivu
            int pageNumber = (page ?? 1); //Tämä palauttaa sivunumeron taikka jos page on null, niin palauttaa numeron yksi
            
            return View(tuotteet.ToPagedList(pageNumber, pageSize));                             //Palautetaan näkymälle lista model.
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