﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Shopf.Models.Data
{
    [Table("tblRoles")]
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}