using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult CS3620()
        {
            return View();
        }

        public ActionResult CS3750()
        {
            return View();
        }

        public ActionResult MATH1220()
        {
            return View();
        }

        public ActionResult PHYS2120()
        {
            return View();
        }

        //**OTHER SECTIONS OF COURSES**

        public ActionResult Grades()
        {
            return View();
        }
    }
}