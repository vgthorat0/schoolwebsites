using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Areas.Admin.Models
{
    public class Event
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [AllowHtml]
        public string Contents { get; set; }
        
    }
}