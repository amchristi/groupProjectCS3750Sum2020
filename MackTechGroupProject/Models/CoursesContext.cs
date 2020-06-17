using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MackTechGroupProject.Models
{
    public class CoursesContext : DbContext
    {
        public CoursesContext() : base()
        {

        }

        public DbSet<Course> Courses { get; set; }
    }
}