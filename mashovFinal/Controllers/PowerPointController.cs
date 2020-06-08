using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace mashovFinal.Controllers
{
    public class PowerPointController : ApiController
    {
      //  bool Local = true;
        [HttpPost]
       // [Route("api/PowerPoint/UploadContent/{ByUser}/{ContentName}")]
        [Route("api/PowerPoint/UploadPowerPoint")]///
        public string UploadPowerPoint()
        {
            //HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            HttpPostedFile file1 = httpRequest.Files[0];

            var p = httpRequest.Form["IDdoc"]+ httpRequest.Form["idGroup"];
            p = p.Replace("|", "\\");
            var idg = Convert.ToInt32( httpRequest.Form["idGroup"]);

            //To save file, use SaveAs method

            var filePath = HttpContext.Current.Server.MapPath("~/PowerPoint\\" + p);
            System.IO.Directory.CreateDirectory(filePath);
            filePath = System.IO.Path.Combine(filePath, file1.FileName);
           

            try 
            {
                file1.SaveAs(filePath);
                //DBservices dbs = new DBservices();
                //dbs.savelink(filePath, idg);
            }
            catch (Exception ex)
            {
                return "שגיאה בהעלאת קובץ";
            }

            return filePath;
        }
        [HttpPut]    
        [Route("api/PowerPoint/sent")]
        public int Putsent([FromBody] Groups d)
        {
            Groups g = new Groups();
            string link = d.Link;
            int idg = d.NumGroup;
            return g.putlink(link,idg);
        }
    }
}
