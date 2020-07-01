using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;


namespace MackTechGroupProject.Controllers
{
    public class AssignmentsController : BaseController
    {
        // GET: Assignments
        public ActionResult Assignment1()
        {
            return View();
        }
        public ActionResult Assignment2()
        {
            return View();
        }
        public ActionResult Assignment3()
        {
            return View();
        }
        public ActionResult Discussion()
        {
            return View();
        }
        public ActionResult AddAssignment()
        {
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddAssignment(Assignment model)
        {      
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    AssignmentId = model.AssignmentId,
                    Course = model.Course,
                    Points = model.Points,
                    AssignmentTitle = model.AssignmentTitle,
                    AssignmentDescription = model.AssignmentDescription,
                    DueDate = model.DueDate,
                    SubmissionType = model.SubmissionType                    
                };

                context.Assignments.Add(assignment);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



    }
}