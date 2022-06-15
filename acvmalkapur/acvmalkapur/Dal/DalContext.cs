using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Dal
{
    public class DalContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<DalContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }
        public DbSet<Event> Event { get; set; }
        public DbSet<Notification> Notification { get; set; }

        public System.Data.Entity.DbSet<Website.Models.Contact> Contacts { get; set; }
    }
    public class Event
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [AllowHtml]
        [Required (ErrorMessage = "Please select image.")]
        public string Image { get; set; }

        [Required]
        [SqlDefaultValue(DefaultValue = "0")]
        public int Priority { get; set; }
    }

    public class Notification
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [AllowHtml]
        public string DisplayText { get; set; }
        [AllowHtml]
        public string Link { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }
    }


}