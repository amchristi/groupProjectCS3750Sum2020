using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class MiscController : Controller
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
    }
}