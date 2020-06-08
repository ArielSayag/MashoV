using mashovFinal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace mashovFinal
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

          CritRanHelper.checkDocs(); //אלמנט חכם

            
            //string str = StringHelper.GeneratePassword();
            //string name = "אריאל";
            //string subject = "סיסמא חדשה למערכת MASHOV , ברוך הבא";
            //string body = "<div style='direction: rtl'>שלום , " + name;
            //body += "<br><br>ברוך הבא למערכת MASHOV, ";
            //body += "<br>שם משתמש שלך הינו: arielsayag19@gmail.com <br> סיסמתך היא: " + str;
            //body += "<br><br>תודה , צוות MASHOV</div>";
            ////SendEMail(item.Email, subject, body);
            //MailHelper.SendEMail("arielsayag19@gmail.com", subject, body);

        }
    }
}
