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
    public class ExcelDocumentController : ApiController
    {

        [HttpPost]
        [Route("api/ExcelDocument")]
      public Dictionary<string, string> Post()
        {
            //HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            HttpPostedFile file1 = httpRequest.Files[0];

            var arr = new string[4];
            for (int i = 0; i < arr.Length; i++)            
                arr[i] = httpRequest.Form["arr"+i];
            

            //To save file, use SaveAs method
            var filePath = HttpContext.Current.Server.MapPath("~/uploadedFiles\\" + file1.FileName);
            file1.SaveAs(filePath); //File will be saved in application root
            try
            {
                ExcelDocument e = new ExcelDocument();

                return e.ReadWorkbook(arr, filePath);
                //  return e.getExcelFile(arr, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ", file path: " + filePath);
            }
            


        }
    }
}
