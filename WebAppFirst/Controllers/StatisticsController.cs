using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppFirst.Models;
using WebAppFirst.ViewModels;
namespace WebAppFirst.Controllers
{
    public class StatisticsController : Controller
    {
        //Tietokantamallin viittaus
        private NorthwindEntities db = new NorthwindEntities();
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        //GET: Gategory Sales
        public ActionResult CategorySales()
        {
            //Label Name - X Koordinaatisto.
            string categoryNameList;
            //Label Data -  Y Koordinaatisto.
            string categorySalesList;
            //Lista - Sisältää tiedot myynnistä.
            List<CategorySalesClass> CategorySalesList = new List<CategorySalesClass>();
            //Datan haku - LINQ
            var categorySalesData = from cs in db.Category_Sales_for_1997
                                    select cs;
            //Datan läpikäynti silmukassa
            foreach (Category_Sales_for_1997 salesfor1997 in categorySalesData)
            {
                //Luodaan yksi myyntirivi
                CategorySalesClass OneSalesRow = new CategorySalesClass();
                //Poimitaan tulleesta datasta kaksi kenttää
                OneSalesRow.CategoryName = salesfor1997.CategoryName;
                OneSalesRow.CategorySales = (int)salesfor1997.CategorySales;
                //Viedään tiedot listalle
                CategorySalesList.Add(OneSalesRow);
            }
            //Viedään tiedot String-muuttujiin
            categoryNameList = "'" + string.Join("','", CategorySalesList.Select(n => n.CategoryName).ToList()) + "'";
            categorySalesList = string.Join(",", CategorySalesList.Select(n => n.CategorySales).ToList());

            //Viedään tiedot ViewBagiin
            ViewBag.categoryName = categoryNameList;
            ViewBag.categorySales = categorySalesList;


            return View();
        }
    }
}