﻿using MackTechGroupProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Stripe;

namespace MackTechGroupProject.Controllers
{
    public class CoursesController : BaseController
    {
        //Add Course
        public ActionResult AddCourse()
        {
            return View();
        }

        // POST: /Courses/AddCourse
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(Course model)
        {
            String userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentInstructor = context.Users.Where(x => x.Id == userId).FirstOrDefault();

            string instructorName = currentInstructor.FirstName + " " + currentInstructor.LastName;

            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseId = model.CourseId,
                    CRN = model.CRN,
                    Department = model.Department,
                    CourseNumber = model.CourseNumber,
                    CourseName = model.CourseName,
                    Instructor = currentInstructor,
                    InstructorName = instructorName,
                    CreditHours = model.CreditHours,
                    ClassLocation = model.ClassLocation,
                    MaxCapacity = model.MaxCapacity
                };

                var instructorEnrollment = new Enrollment
                {
                    Course = course,
                    User = currentInstructor
                };

                context.Courses.Add(course);
                context.Enrollments.Add(instructorEnrollment);
                context.SaveChanges();

                currentEnrollments.Add(instructorEnrollment);

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult CourseRegistration()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            var allCourses = context.Courses.ToList();

            return View(allCourses);
        }

        public ActionResult RegisterForCourse(int id)
        {
            String userId = User.Identity.GetUserId();
            var selectedCourseId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var currentStudent = context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var studentEnrollment = new Enrollment
                {
                    Course = selectedCourse,
                    User = currentStudent
                };

                //var currentEnrollments = context.Enrollments.Include(x => x.User).Include(c => c.Course).Where(s => s.User.Id == userId).ToList();

                //check to see if the course already exists in student's currentEnrollments
                bool hasCourse = currentEnrollments.Any(x => x.Course.CourseId == id);

                if (!hasCourse)
                {
                    context.Enrollments.Add(studentEnrollment);
                    context.SaveChanges();

                    currentEnrollments.Add(studentEnrollment);

                    return RedirectToAction("StudentAccount", "Courses");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error: You are already registered for this course";
                    return RedirectToAction("CourseRegistration", "Courses");
                }
            }

            //don't want to get this far
            return RedirectToAction("CourseRegistration", "Courses");
        }

        public ActionResult DeleteCourseFromEnrollments(int id)
        {
            String userId = User.Identity.GetUserId();
            var selectedEnrollmentId = id;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            // query current enrollments list and delete selected course where the selectedCourseId == x.Course.CourseI
            var currentEnrollmentList = currentEnrollments.Where(x => x.EnrollmentId == selectedEnrollmentId).FirstOrDefault();
            
            //currentEnrollments.Remove(selectedCourse);
            currentEnrollments.Remove(currentEnrollmentList);

            //DELETE from database as well
            var currentEnrollmentDB = context.Enrollments.Where(x => x.EnrollmentId == selectedEnrollmentId).FirstOrDefault();
            context.Enrollments.Remove(currentEnrollmentDB);
            context.SaveChanges();
            

            return RedirectToAction("StudentAccount", "Courses");
        }

        public ActionResult StudentAccount()
        {
            String userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            // gets a list of enrollments for current student
            var currentEnrollments = context.Enrollments.Include(x => x.User).Include(c => c.Course).Where(s => s.User.Id == userId).ToList();

            var totalCreditHours = currentEnrollments.Sum(x => x.Course.CreditHours);
            var totalCost = totalCreditHours * 240;
            var formattedCost = totalCost.ToString("C0");

            ViewBag.TotalCreditHours = totalCreditHours;
            ViewBag.TotalFormattedCost = formattedCost;

            return View(currentEnrollments);
        }

        public ActionResult TuitionPayment()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        // GET: Payment
        public async Task<ActionResult> TuitionPayment(PaymentInfo model)
        {
            if (ModelState.IsValid)
            {
                var paymentInfo = new PaymentInfo
                {
                    FullName = model.FullName,
                    CreditCardNumber = model.CreditCardNumber,
                    ExpirationMonth = model.ExpirationMonth,
                    ExpirationYear = model.ExpirationYear,
                    CVC = model.CVC,
                    PaymentAmount = model.PaymentAmount 
                };

                // STRIPE API
                var url = "https://api.stripe.com/";
                var stripeTokenUrl = url + "v1/tokens";
                var stripeChargesUrl = url + "v1/charges";

                //used to store stripe response as object
                dynamic tokenData = null;
                dynamic chargesData = null;

                var stripeAmount = paymentInfo.PaymentAmount;

                // used to store test credit card information
                var ccparams = new Dictionary<string, string>();
                // set ccparams
                ccparams.Add("card[number]", "4242424242424242");
                ccparams.Add("card[exp_month]", "7");
                ccparams.Add("card[exp_year]", "2021");
                ccparams.Add("card[cvc]", "314");

                // used to store charge information
                var chrgparams = new Dictionary<string, string>();
                // set chrgparams
                chrgparams.Add("amount", stripeAmount.ToString("0.00").Replace(".",string.Empty));
                chrgparams.Add("currency", "usd");
                chrgparams.Add("source", "tok_visa");
                chrgparams.Add("description", "Tuition Payment Testing");

                // First POST to Stripe
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", STRIPE_KEY);
                var request = new HttpRequestMessage(HttpMethod.Post, stripeTokenUrl) { Content = new FormUrlEncodedContent(ccparams) };
                var response = await client.SendAsync(request);
                string result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    //deserialize json
                    tokenData = JObject.Parse(result);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error: Cannot connect with Stripe API";
                    return RedirectToAction("TuitionPayment", "Courses");
                }

                // Second POST to Stripe
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SECRET_KEY);
                request = new HttpRequestMessage(HttpMethod.Post, stripeChargesUrl) { Content = new FormUrlEncodedContent(chrgparams) };
                response = await client.SendAsync(request);
                result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    //deserialize json
                    chargesData = JObject.Parse(result);

                    TempData["SuccessMessage"] = "Payment has been posted!";
                    return RedirectToAction("TuitionPayment", "Courses");
                }
                else
                {
                    TempData["Message"] = "Error: Cannot connect with Stripe API";
                    return RedirectToAction("TuitionPayment", "Courses");
                }


                //var content = new FormUrlEncodedContent(ccparams);
                //var response = client.PostAsync(stripeCardUrl, content).Result;
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("TuitionPayment", "Courses");
        }

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

        public ActionResult CourseDetail()
        {
            return View();
        }

    }
}