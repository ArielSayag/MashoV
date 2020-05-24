using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;
using mashovFinal.Utilities;


namespace mashovFinal.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPut]
        [Route("api/Login/User")]
        public Users Put([FromBody]Users u)
        {
            
            return u.ifexist();
        }
        [HttpPut]
        [Route("api/Login/Student")]
        public Students PutStudent([FromBody]Students s)
        {
            
            return s.checkingExist();
        }
        [HttpPut]
        [Route("api/Login/Pass")]
        public string PutPass([FromBody]Users u)
        {

            Users u1=u.ifexist();
            if (u1.Typesofuser.Count>0)
            {
                string body = @"סיסמתך היא: " + u1.Pass;
                MailHelper.SendEMail(u1.Email,"שחזור סיסמא", body);
               // MailHelper.SendEMail("arielsayag19@gmail.com", "שחזור סיסמא", body);
            }
            return "1";
        }
        [HttpPut]
        [Route("api/Login/PassStudent")]
        public string PutPassStudent([FromBody]Students s)
        {
            Students s1= s.checkingExist();
            if (s1.Id != "")
            {
                if (s1.Email != "")
                {
                    string body = @"סיסמתך היא: " + s1.Pass;
                    MailHelper.SendEMail(s1.Email, "שחזור סיסמא", body);
                    return "1";
                }
                else
                {
                    return s1.Id;                 
                }
            }
            else
            {
                return "0"; 
            }
          
        }
        [HttpPost]
        [Route("api/Login/Email")]
        public int Post([FromBody]Students s)
        {
            
             Students afterupdate=s.updateEmail();

            string body = @"סיסמתך היא: " + afterupdate.Pass;
            MailHelper.SendEMail(afterupdate.Email, "שחזור סיסמא", body);
            return 1;

         
        }

        //-----------updateUser------------//
        
        [HttpPost]
        [Route("api/Login/updateuser")]
        public int PostupdateuserorS([FromBody]Users s)
        {

            return s.updateUsers();

        }
        [HttpPost]
        [Route("api/Login/updateS")]
        public int PostupdateuserorS([FromBody]Students s)
        {

            return s.updateStudent();

        }
    }
}
