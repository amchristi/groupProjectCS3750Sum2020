using MackTechGroupProject.Models;
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
using MackTechGroupProject.BusinessLogic;

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

                //Does this Course Already Exist?
                //bool hasCourse = RegistrationService.hasCourse(course, userId, context);

                //If the course already exists delete the old one. 
                //if(hasCourse)
                //{
                //    RegistrationService.DeleteDuplicateCourse(course, userId, context);
                //}

                //context.Courses.Add(course);
                bool result = RegistrationService.addNewCourse(course, context);

                if (result) //check if course was successfully added
                {
                    context.Enrollments.Add(instructorEnrollment);
                    context.SaveChanges();
                    currentEnrollments.Add(instructorEnrollment);
                }

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
            if (ModelState.IsValid)
            {

                String userId = User.Identity.GetUserId();
                var selectedCourseId = id;
                var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

                Boolean result = RegistrationService.AddCourseService(userId, selectedCourseId, currentEnrollments, context);

                if (result)
                {
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

            RegistrationService.DeleteCourseService(selectedEnrollmentId, currentEnrollments, context);

            return RedirectToAction("StudentAccount", "Courses");
        }

        public ActionResult StudentAccount()
        {
            String userId = User.Identity.GetUserId();
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            decimal CREDIT_HOUR_COST = 240;

            // gets a list of enrollments for current student
            var currentEnrollments = context.Enrollments.Include(x => x.User).Include(c => c.Course).Where(s => s.User.Id == userId).ToList();
            // get currentStudent
            var currentStudent = context.Users.Where(x => x.Id == userId).FirstOrDefault();

            // gets sum of all credit hours enrolled in
            var totalCreditHours = currentEnrollments.Sum(x => x.Course.CreditHours);
            ViewBag.TotalCreditHours = totalCreditHours;

            var totalCostofCreditHours = totalCreditHours * CREDIT_HOUR_COST;

            // if the student already exists in the accounting table
            if (context.Accounting.Any(x => x.User.Id == userId))
            {
                // gets current student account from Accounting table
                var currentStudentAccount = context.Accounting.Where(x => x.User.Id == userId).FirstOrDefault();
                
                // get the difference in credit hours
                var diffInCreditHours = (totalCreditHours - currentStudentAccount.TotalCreditHours);

                // get new cost needed to be added or subtracted to account
                decimal newCreditCosts = (diffInCreditHours * CREDIT_HOUR_COST);

                var newTotalBalance = (currentStudentAccount.TotalBalance + newCreditCosts);
                // update total cost in database
                currentStudentAccount.TotalBalance = newTotalBalance;

                // update credit hours in database
                currentStudentAccount.TotalCreditHours = totalCreditHours;
                context.SaveChanges();


                // the user already exists, get their balance
                var formattedCost = String.Format("{0:C2}", currentStudentAccount.TotalBalance);

                ViewBag.TotalFormattedCost = formattedCost;
            }
            else
            {
                var accounting = new Accounting()
                {
                    User = currentStudent,
                    PaymentDate = DateTime.Now,
                    TotalCreditHours = totalCreditHours,
                    TotalBalance = totalCostofCreditHours
                };
                var formattedCost = String.Format("{0:C2}", accounting.TotalBalance);

                context.Accounting.Add(accounting);
                context.SaveChanges();

                ViewBag.TotalFormattedCost = formattedCost;
            }

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
            var userId = User.Identity.GetUserId();
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

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

                    // update totalcost in accounting table
                    // get the student account using context then save total cost
                    var currentStudentAccount = context.Accounting.Where(x => x.User.Id == userId).FirstOrDefault();
                    var totalBalance = currentStudentAccount.TotalBalance;
                    var newTotalBalance = totalBalance - paymentInfo.PaymentAmount;

                    currentStudentAccount.TotalBalance = newTotalBalance;
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Payment has been posted! Check your account balance.";
                    return RedirectToAction("TuitionPayment", "Courses");
                }
                else
                {
                    TempData["Message"] = "Error: Cannot connect with Stripe API";
                    return RedirectToAction("TuitionPayment", "Courses");
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("TuitionPayment", "Courses");
        }

        public ActionResult CourseDetail(int id)
        {
            var selectedCourseId = id;
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var selectedCourse = context.Courses.Where(x => x.CourseId == selectedCourseId).Include(x => x.Assignments).FirstOrDefault();

            return View(selectedCourse);
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
            var userId = User.Identity.GetUserId();

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //var courseAssignments = context.Assignments.Where(x => x.Course.CourseId == selectedCourseId).ToList();

            var courseIds = currentEnrollments.Where(x => x.User.Id == userId).Select(x => x.Course).Select(y => y.CourseId).ToList();

            foreach (var courseId in courseIds)
            {
                //get a list of all submission for courseId
                var studentGrades = context.SubmissionGrades.Include(x => x.User).Include(x => x.Assignment).Include("Assignment.Course")
                                        .Where(x => x.User.Id == userId && x.Assignment.Course.CourseId == courseId && x.Grade != null).ToList();

                //get a total points for courseId where assignments are graded
                var pointsTotal = studentGrades.Sum(x => x.Assignment.Points);

                //get user grade
                var gradeTotal = studentGrades.Sum(x => x.Grade);

                // set Percentage of user using usergrade
                currentEnrollments.Where(x => x.Course.CourseId == courseId).FirstOrDefault().Course.Percentage = Convert.ToDecimal(gradeTotal / pointsTotal);

            }

            var ListOfEnrollments = new ListOfEnrollmentsViewModel()
            {
                Enrollments = currentEnrollments
            };

            return View(ListOfEnrollments);
        }
    }
}