using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MackTechGroupProject
{
    public class MvcApplication : System.Web.HttpApplication { 
    
        string connString = ConfigurationManager.ConnectionStrings
        ["TitanDbConnection"].ConnectionString;


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SqlDependency.Start(connString);
        }

        protected void Session_start(object sender, EventArgs e)
        {
            NotificationComponents NC = new NotificationComponents();
            var currentTime = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = currentTime;
            NC.UpdatedAssignmentGradeNotification(currentTime);
            NC.AddedAssignmentNotification(currentTime);
        }

        protected void Application_End()
        {
            SqlDependency.Stop(connString);
        }
    }

}
