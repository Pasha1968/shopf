using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Shopf.Models.Data
{
    [Table("tblOrders")]
    public class OrderDTO
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Createdat { get; set; }
        public string Status { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO Users { get; set; }

    }
}