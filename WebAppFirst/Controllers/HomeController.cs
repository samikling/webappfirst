using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppFirst.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Tietoa minusta:";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Täältä löydät yhteystietoni.";

            return View();
        }
        public ActionResult Map()
        {
            ViewBag.Message = "Saapumisohje";

            return View();
        }
    }
}