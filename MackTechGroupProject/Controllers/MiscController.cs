using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class MiscController : BaseController
    {
        // GET: Misc
        public ActionResult Registration()
        {
            return View();
        }
        public ActionResult Message()
        {
            return View();
        }
        public ActionResult Calendar()
        {
            return View();
        }
        public ActionResult NewEvent()
        {
            return View();
        }

        public ActionResult Account()
        {
            return View();
        }

        public ActionResult BillPay()
        {
            return View();
        }

    }
}