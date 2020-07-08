using MackTechGroupProject.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class BaseController : Controller
    {
        public static HttpClient client = new HttpClient();
        public static string STRIPE_KEY = "pk_test_51GumXVFsxre9bmJMuCAoLJIQfiicW6silz0Iur2IDvvwIQyCfeI4ViuaIMduRbycyRVzf0kav8PL1rzJo3wrxoc200cSwquuz9";
        public static string SECRET_KEY = "sk_test_51GumXVFsxre9bmJM11BoRDH7jG3HvuIZWiv7qKOZzbchuA2EQu6XEbKxCncDLeKwCATWf1hHTd7hGUMckkdXHYFe00PNxwrWUZ";
        public static List<Enrollment> currentEnrollments { get; set; }

        // GET: Base
        public void GetCurrentEnrollments(string userEmail)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            currentEnrollments = context.Enrollments.Where(x => x.User.Email == userEmail).Include(x => x.User).Include(x => x.Course).ToList();
        }
    }
}