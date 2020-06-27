using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View(currentEnrollments);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}