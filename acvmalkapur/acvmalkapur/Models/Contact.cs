using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class Contact
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]
        [RegularExpression(@"[789]\d{9}$", ErrorMessage = "Invalid Phone number")]
        public string Mobile { get; set; }
        [Required]
        public string Message { get; set; }
    }
}