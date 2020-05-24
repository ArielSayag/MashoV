using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.Mvc;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace mashovFinal.Models
{
    public class CreateExcelFile
    {

        public void createWorkbook(int d)
        {
            DBservices dbs = new DBservices();
            List<Students> listFinal = new List<Students>();

            //int index = d.NumDoc;
            listFinal = dbs.getFinalScore(d);
            IWorkbook workbook = new XSSFWorkbook();
            // string subject = @" ציונים סופיים בקורס " + d.DetailsMeet.DetailsCourseDep.NameCourse + " במחלקה "
            //        + d.DetailsMeet.DetailsCourseDep.NameDepartment + " במצגת " + d.NameDoc;
            string subject = "final";
            ISheet sheet = workbook.CreateSheet(subject);
            //
            //

            //firstHeadeline//
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
               // double score = account.FinalScore * (weight / 100);
               // row.CreateCell(4).SetCellValue(score.ToString());
                // row.CreateCell(2).SetCellValue(account.CreationDate.ToShortDateString());
                // row.CreateCell(3).SetCellValue(account.LastLoginDate.ToShortDateString());
                // row.CreateCell(4).SetCellValue(account.IsApproved);
                // row.CreateCell(5).SetCellValue(account.Comment);
                rowIndex++;
            }

            for (int i = 0; i <= rowIndex; i++) sheet.AutoSizeColumn(i);
            
            System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
            //using (var exportData = new MemoryStream())
            string s = subject + "-{0:d}.xls";
            string saveAsFileName = string.Format(s, DateTime.Now).Replace("/", "-");

            string path = HttpContext.Current.Server.MapPath("~/" + saveAsFileName);

            if (!Directory.Exists(Path.GetFullPath(path)))
            {
                Directory.CreateDirectory(path);
            }
            //using (var exportData = new FileStream(path, FileMode.Create))
            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", saveAsFileName));
                Response.Clear();
                Response.BinaryWrite(exportData.GetBuffer());
                //Response.TransmitFile(path);
                Response.End();
               
            }
        }
    }
}