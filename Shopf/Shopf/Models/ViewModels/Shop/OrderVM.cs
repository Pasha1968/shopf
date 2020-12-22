using Shopf.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopf.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM() {
        }
        public OrderVM(OrderDTO row)
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            Cratedat = row.Cratedat;
            Status = row.Status;
        }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime Cratedat { get; set; }
        public string Status { get; set; }
    }
}