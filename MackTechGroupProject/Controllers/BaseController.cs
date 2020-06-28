using MackTechGroupProject.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class BaseController : Controller
    {
        public static List<Enrollment> currentEnrollments { get; set; }

        // GET: Base
        public void GetCurrentEnrollments(string userEmail)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            currentEnrollments = context.Enrollments.Where(x => x.User.Email == userEmail).Include(x => x.User).Include(x => x.Course).ToList();
        }
    }
}