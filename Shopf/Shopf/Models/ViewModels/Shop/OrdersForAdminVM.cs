using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopf.Models.ViewModels.Shop
{
    public class OrdersForAdminVM
    {
        public int OrderNumber { get; set; }
        public string UserName { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string,int> ProductsAndQty { get; set; }
        public DateTime Cratedat { get; set; }
        public String Status { get; set; }
    }
}