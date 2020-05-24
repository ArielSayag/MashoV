using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using mashovFinal.Models;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using mashovFinal.Utilities;
using System.Data;

namespace mashovFinal.Controllers
{
    public class CreateExcelController : Controller
    {

        [HttpGet]
        public ActionResult PostReportPartial(int index)
        {
            DBservices dbs = new DBservices();
            List<Students> listFinal = new List<Students>();

           // int index = d.NumDoc;
            listFinal = dbs.getFinalScore(index);
            IWorkbook workbook = new HSSFWorkbook();
            //string subject = @" ציונים סופיים בקורס " + d.DetailsMeet.DetailsCourseDep.NameCourse + " במחלקה "
            //                 + d.DetailsMeet.DetailsCourseDep.NameDepartment + " במצגת " + d.NameDoc;
            string subject = @" ציונים סופיים בקורס"; /*+ d.NameDoc;*/
            ISheet sheet = workbook.CreateSheet(subject);
         


            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("שם פרטי");
            row.CreateCell(1).SetCellValue("שם משפחה");
            row.CreateCell(2).SetCellValue("תעודת זהות");
            row.CreateCell(3).SetCellValue("ציון סופי");
            //    row.CreateCell(4).SetCellValue("ציון משוקלל");

            rowIndex++;

            foreach (var account in listFinal)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(account.FirstName);
                row.CreateCell(1).SetCellValue(account.LastName);
                row.CreateCell(2).SetCellValue(account.Id);
                row.CreateCell(3).SetCellValue(account.FinalScore.ToString());
          
                rowIndex++;
            }

            for (int i = 0; i <= rowIndex; i++) sheet.AutoSizeColumn(i);


            // Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();
            MemoryStream ms = new MemoryStream();
            using (NpoiMemoryStream memoryStream = new NpoiMemoryStream())
            {
                memoryStream.AllowClose = false;
                workbook.Write(memoryStream);
                TempData[handle] = memoryStream.ToArray();
                // ms.Write(TempData[handle], true, 1);
                //memoryStream.Position = 0;

                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.AllowClose = true;

            }

            // Note we are returning a filename as well as the handle
            return new JsonResult()
            {

                Data = new { FileGuid = handle, FileName = Path.Combine(subject, ".xls") }
            };

     


        }
        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {

            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }


        }
        [HttpGet]
        public virtual ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/MyFiles"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }
        // double score = account.FinalScore * (weight / 100);
        // row.CreateCell(4).SetCellValue(score.ToString());
        // row.CreateCell(2).SetCellValue(account.CreationDate.ToShortDateString());
        // row.CreateCell(3).SetCellValue(account.LastLoginDate.ToShortDateString());
        // row.CreateCell(4).SetCellValue(account.IsApproved);
        // row.CreateCell(5).SetCellValue(account.Comment);
    }
}
