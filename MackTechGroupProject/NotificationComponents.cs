using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.AspNet.SignalR;
using MackTechGroupProject.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;

namespace MackTechGroupProject
{
    public class NotificationComponents
    {
        //here we will add a function for register notification (will add sql dependency)
        public void RegisterNotification(DateTime currentTime)
        {
            string conStr = ConfigurationManager.ConnectionStrings["TitanDbConnection"].ConnectionString;
            //string sqlCommand = @"SELECT [Grade] from [dbo].[SubmissionGrades] where [GradeAddedOn] > @GradeAddedOn";
            string sqlCommand = @"SELECT AssignmentTitle, Points, Grade from [dbo].[SubmissionGrades] sg INNER JOIN [dbo].[Assignments] a
                                    ON sg.Assignment_AssignmentId = a.AssignmentId
                                    where sg.GradeAddedOn > @GradeAddedOn";
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@GradeAddedOn", currentTime);
                if(con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);
                //sqlDep.OnChange += sqlDep_OnChange;
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    //nothing need to add here
                }

            }
        }

        void sqlDep_OnChange(Object sender, SqlNotificationEventArgs e)
        {
            if( e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                //from here we will send message to client
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");

                //re-register notifcation
                RegisterNotification(DateTime.Now);
            }
        }
        
        public List<GradedAssignmentViewModel> GetGradedAssignments(DateTime afterDate, string userId)
        {
            
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                
                List<GradedAssignmentViewModel> result = new List<GradedAssignmentViewModel>();
                var updatedSubmissions = context.SubmissionGrades.Where(x => x.GradeAddedOn > afterDate).Include(x => x.Assignment).Include(x => x.User).OrderByDescending(x => x.GradeAddedOn).ToList();
                foreach(SubmissionGrades sg in updatedSubmissions)
                {
                    if(sg.User.Id == userId)
                    {
                        GradedAssignmentViewModel newGraded = new GradedAssignmentViewModel()
                        {
                            AssignmentTitle = sg.Assignment.AssignmentTitle,
                            Grade = (double) sg.Grade,
                            Points = sg.Assignment.Points
                        };
                        result.Add(newGraded);
                    }
                }

                return result;
            }
            
        } 
    }
}