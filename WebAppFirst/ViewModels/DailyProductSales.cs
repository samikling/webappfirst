using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppFirst.ViewModels
{
    public class DailyProductSales
    {
        public string OrderDate { get; set; }
        public string ProductName { get; set; }
        public float DailySales { get; set; }
    }
}