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
        public HttpResponseMessage UploadPowerPoint()
        {
           // Content content = new Content();
           //List<string> imageLinks = new List<string>();
            var httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count > 0)
            {
                //Loop through uploaded files  
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];

                    // this is an example of how you can extract addional values from the Ajax call
                    string name = httpContext.Request.Form["myfile"];
                    int idG= Convert.ToInt32( httpContext.Request.Form["idG"]);

                    if (httpPostedFile != null)
                    {
                        // Construct file save path  
                       // string fname = $@"{ContentName}-{ByUser}.{httpPostedFile.FileName.Split('\\').Last().Split('.').Last()}";//holocaust-shiftan92.pptx

                        string fname = $@"{httpPostedFile.FileName.Split('\\').Last().Split('.').Last()}";

                        var fileSavePath = "";
                        //if (Local)
                        //{
                            //fileSavePath = Path.Combine(UrlLocal, fname);//אם עובדים לוקלי ישמור תמונות בתיקיית פבליק של הקליינט
                        //}
                        //else
                        //{
                            fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/PowerPoint"), fname);//אם עובדים על השרת שומרים תמונות בתיקייה של השרת
                        //}
                        // Save the uploaded file  
                        httpPostedFile.SaveAs(fileSavePath);

                        try
                        {
                            DBservices dbs = new DBservices();
                            dbs.savelink(fileSavePath,idG);
                        }
                        catch
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, "שגיאה בהעלאת קובץ");
                        }
                        //פיצול מצגת לתמונות
                        //using (Aspose.Slides.Presentation pres = new Aspose.Slides.Presentation(fileSavePath))
                        //{
                        //    int countPages = 0;
                        //    foreach (ISlide sld in pres.Slides)
                        //    {
                        //        if (Local)
                        //        {

                        //            // Create a full scale image
                        //            Bitmap bmp = sld.GetThumbnail(1f, 1f);

                        //            fileSavePath = Path.Combine(UrlLocal, string.Format("{0}-{1}{2}.jpg", ContentName, ByUser, sld.SlideNumber));// $@"{Email.Split('@').First()}{sld.SlideNumber}"
                        //                                                                                                                         // Save the image to disk in JPEG format
                        //            bmp.Save(fileSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //        }
                        //        else
                        //        {
                        //            // Create a full scale image
                        //            Bitmap bmp = sld.GetThumbnail(1f, 1f);
                        //            //fileSavePath = Path.Combine(UrlServer, string.Format("{0}-{1}{2}.jpg", ContentName, ByUser, sld.SlideNumber));// $@"{Email.Split('@').First()}{sld.SlideNumber}"
                        //            fileSavePath = Path.Combine(HostingEnvironment.MapPath("~/uploadedContents"), string.Format("{0}-{1}_{2}.jpg", ContentName, ByUser, sld.SlideNumber));                                                                                                                 // Save the image to disk in JPEG format
                        //            bmp.Save(fileSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //        }
                        //        countPages++;
                        //    }
                        //    //עדכון מספר עמודים של התוכן בדטה בייס
                        //    content = content.UpdatePages(countPages);
                        //}

                    }
                }
                // Return status code  
                return Request.CreateResponse(HttpStatusCode.Created /*,content*/);

            }
            return Request.CreateResponse(HttpStatusCode.Created, "שגיאה בהעלאת קובץ");
        }
    }
}
