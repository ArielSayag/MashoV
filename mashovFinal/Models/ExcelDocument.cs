using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Net.Mail;

namespace mashovFinal.Models
{
    public class ExcelDocument
    {
        public object MessageBoxButton { get; private set; }
        public object MessageBox { get; private set; }
        public bool DialogResult { get; private set; }
        public object MessageBoxImage { get; private set; }
        //public int getExcelFile(string path, string nameOfDepartment ,string nameOfCourse/*, DateTime hebrewYear*/, string feedbackName)
        public FeedBack_Doc getExcelFile(string[] path, string p)
        {
            //string p = @"D:\" + path[3];

            //string fname = httpPostedFile.FileName.Split('\\').Last();
            string fname = path[3];
            //  var p = Path.Combine(HostingEnvironment.MapPath("~/uploadedFiles"), fname);

            // check if file exists
            if (!File.Exists(p))
                throw new FileNotFoundException();

            //Create COM Objects. Create a COM object for everything that is referenced
            Application xlApp;
            Workbook xlWorkbook;
            try
            {
                xlApp = new Excel.Application();

                xlWorkbook = xlApp.Workbooks.Open(p); // exception is thrown here
                //  Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                //  Excel.Range xlRange = xlWorksheet.UsedRange;
            }
            catch (Exception e)
            {
                // can't load file to excel application :\
                throw new Exception(e.Message + ", file path: " + p);

            }

            int numEffected = 0;

            List<Users> allUsers = new List<Users>();
            List<Students> allStudents = new List<Students>();
            List<Groups> allGroups = new List<Groups>();
            List<Group_Meeting> allmeetings = new List<Group_Meeting>();


            HebrewCalendar HebCal = new HebrewCalendar();
            //string yearHeb = HebCal.GetYear(DateTime.Now.ToString(string));
            //var currentCulture = CultureInfo.CurrentCulture; // just for reference
            var culture = CultureInfo.CreateSpecificCulture("he-IL"); // proper "Hebrew" culture
            culture.DateTimeFormat.Calendar = HebCal;
            Thread.CurrentThread.CurrentCulture = culture;
            string minYearHeb = HebCal.MinSupportedDateTime.ToString("yyyy"); // min supported year
            string yearHeb = DateTime.Now.ToString("yyyy", culture); // current year
            string heb = yearHeb.Replace("\"", "`");

            FeedBack_Meeting m = new FeedBack_Meeting
            {
                YearMeeting = heb,
                NameMeeting = path[0]

            };

            // m.DetailsCourseDep is null!
            m.DetailsCourseDep = new CoursesAndDepartment();
            m.DetailsCourseDep.NumDepartment = int.Parse(path[1]);
            m.DetailsCourseDep.NumCourse = int.Parse(path[2]);

            var connection = new Dictionary<double, string>();

            FeedBack_Doc doc = new FeedBack_Doc();
            doc.NameDoc = path[0];
            doc.Manager = new List<Users>();

            int sheetCount = xlWorkbook.Sheets.Count;
            for (int a = 0; a < sheetCount; a++)
            {
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[a + 1];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;



                List<string> firstRow = new List<string>();

                for (int fr = 1; fr <= colCount; fr++)
                {
                    if (xlRange.Cells[1, fr].Value2 != null)
                        firstRow.Add(xlRange.Cells[1, fr].Value2.Trim());
                }



                string team = "";
                bool emteyTeam = false;

                bool oneDate = false;


                for (int i = 2; i <= rowCount; i++)
                {
                    Students s = new Students();
                    Users u = new Users();
                    u.Type = new Types();
                    Users manager = new Users();
                    manager.Type = new Types();
                    Users jug = new Users();
                    jug.Type = new Types();


                    Groups g = new Groups();
                    g.Mentor = new Users();
                    g.Mentor.Type = new Types();
                    Group_Meeting jg = new Group_Meeting();
                    jg.JudgesGroup = new List<Judge_Group_Meeting>();
                    jg.Group = new Groups();
                    DateTime thisdate = new DateTime();



                    for (int j = 1; j <= colCount; j++)
                    {
                        if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                        {
                            switch (firstRow[j - 1])
                            {
                                case "צוות מספר":
                                    if (xlRange.Cells[i, j].Value2 == null)
                                    {
                                        emteyTeam = true;
                                        s.GroupS = new List<Groups>();
                                        Groups gs = new Groups();
                                        gs.NameGroup = team;
                                        s.GroupS.Add(gs);


                                    }
                                    else
                                    {

                                        emteyTeam = false;
                                        g.NameGroup = xlRange.Cells[i, j].Value2.ToString();
                                        team = g.NameGroup;

                                        s.GroupS = new List<Groups>();
                                        Groups gs = new Groups();
                                        gs.NameGroup = team;
                                        s.GroupS.Add(gs);
                                        jg.Group = new Groups();
                                        jg.Group.NameGroup = team;


                                        // jg.NumGroup = team;
                                        //s.NumGroup = team;

                                    }
                                    break;

                                case "שם הסטודנט":
                                    s.FirstName = xlRange.Cells[i, j].Value2.Trim();
                                    var ifem = xlRange.Cells[i, 1].Value2;
                                    if (ifem == null)
                                    {
                                        emteyTeam = true;
                                        // s.NumGroup = team;
                                        s.GroupS = new List<Groups>();
                                        Groups gs = new Groups();
                                        gs.NameGroup = team;
                                        s.GroupS.Add(gs);
                                    }

                                    break;

                                case "שם משפחה":
                                    s.LastName = xlRange.Cells[i, j].Value2.ToString();
                                    break;

                                case "ת.ז":
                                    s.Id = xlRange.Cells[i, j].Value2.ToString();
                                    break;

                                case "כתובת מייל":
                                    u.Email = xlRange.Cells[i, j].Value2.ToString();
                                    // u.Type.NumType = 4;
                                    break;
                                case "שם משתמש":
                                    u.FirstName = xlRange.Cells[i, j].Value2.ToString();
                                    break;
                                case "שם משפחה משתמש":
                                    u.LastName = xlRange.Cells[i, j].Value2.ToString();
                                    break;

                                case "מנהל":
                                    if (xlRange.Cells[i, j].Value2.ToString() == "כן")
                                    {
                                        manager.FirstName = u.FirstName;
                                        manager.LastName = u.LastName;
                                        manager.Email = u.Email;
                                        manager.Type.NumType = 2;
                                        doc.Manager.Add(manager);

                                    }

                                    break;
                                case "שופט":
                                    if (xlRange.Cells[i, j].Value2.ToString() == "כן")
                                    {

                                        jug.FirstName = u.FirstName;
                                        jug.LastName = u.LastName;
                                        jug.Email = u.Email;
                                        jug.Type.NumType = 3;


                                    }

                                    break;
                                case "מנחה":
                                    if (xlRange.Cells[i, j].Value2.ToString() == "כן")
                                    {
                                        u.Type.NumType = 4;

                                    }

                                    break;


                                case "מס מנחה":
                                    if (emteyTeam == false)
                                    {
                                        var key = xlRange.Cells[i, j].Value2;
                                        if (connection.ContainsKey(key))
                                        {
                                            g.Mentor.Email = connection[key];
                                            g.Mentor.Type.NumType = 4;
                                        }


                                    }
                                    break;


                                case "מס שופט":
                                    if (emteyTeam == false)
                                    {

                                        Judge_Group_Meeting ju = new Judge_Group_Meeting();
                                        ju.Judge = new Users();
                                        ju.Judge.Type = new Types();
                                        var key = xlRange.Cells[i, j].Value2;
                                        if (connection.ContainsKey(key))
                                        {
                                            ju.Judge.Email = connection[key];
                                            ju.Judge.Type.NumType = 3;
                                            jg.JudgesGroup.Add(ju);
                                            int coun = 1;
                                            while (xlRange.Cells[i + coun, j].Value2 != null)
                                            {
                                                if (xlRange.Cells[i + coun, j].Value2 != null && xlRange.Cells[i + coun, 1].Value2 == null)
                                                {
                                                    if (connection.ContainsKey(key))
                                                    {
                                                        ju.Judge.Email = connection[key];
                                                        ju.Judge.Type.NumType = 3;
                                                        jg.JudgesGroup.Add(ju);
                                                    }

                                                }
                                                coun++;
                                            }
                                        }

                                    }
                                    break;

                                case "שם הפרויקט":
                                    if (emteyTeam == false)
                                    {
                                        string name = xlRange.Cells[i, j].Value2.ToString();
                                        string newName = name.Replace("\"", "`");
                                        g.NameProject = newName;


                                    }
                                    break;

                                case "ארגון":
                                    if (emteyTeam == false)
                                    {
                                        string name = xlRange.Cells[i, j].Value2.ToString();
                                        string newName = name.Replace("\"", "`");
                                        g.NameOrganization = newName;
                                    }
                                    break;

                                case "סוג הפרויקט":
                                    if (emteyTeam == false)
                                    {
                                        g.ProjectType = xlRange.Cells[i, j].Value2.ToString();
                                    }
                                    break;

                                case "שעת התחלה":
                                    if (emteyTeam == false)
                                    {
                                        jg.StartTime = DateTime.FromOADate(xlRange.Cells[i, j].Value2).ToString("HH:mm");
                                        // jg.StartTime = (DateTime)xlRange.Cells[i, j].Value2;
                                    }
                                    break;
                                case "שעת סיום":
                                    if (emteyTeam == false)
                                    {
                                        jg.EndTime = DateTime.FromOADate(xlRange.Cells[i, j].Value2).ToString("HH:mm");
                                    }
                                    break;
                                case "מס רץ":
                                    double x = xlRange.Cells[i, j].Value2;
                                    string y = xlRange.Cells[i, j + 1].Value2.ToString();
                                    connection.Add(x, y);

                                    break;
                                case "תאריך":
                                    if (oneDate == false)
                                    {
                                        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-IL");
                                        DateTime date = DateTime.FromOADate(xlRange.Cells[i, j].Value2);
                                        //  date.ToString("dd/MM/yyyy");

                                        m.Date = date;
                                        thisdate = m.Date;
                                        oneDate = true;

                                    }
                                    else
                                    {
                                        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-IL");
                                        DateTime date1 = DateTime.FromOADate(xlRange.Cells[1, 13].Value2);
                                        m.Date = date1;
                                    }
                                    break;
                            }
                        }
                    }
                    //ברגע שסיימתי לעבור על שורה
                    if (s.Id != null)
                    {
                        allStudents.Add(s);
                    }
                    if (u.Type.NumType != 0)
                    {
                        allUsers.Add(u);
                    }
                    if (jug.Email != null)
                    {
                        allUsers.Add(jug);
                    }
                    if (manager.Email != null)
                    {
                        allUsers.Add(manager);
                    }
                    if (g.NameGroup != null)
                    {
                        allGroups.Add(g);
                    }
                    if (jg.Group.NameGroup != null)
                    {
                        allmeetings.Add(jg);
                    }
                }

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);

            }
            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            DBservices dbs = new DBservices();
            if (allUsers.Count > 0)
            {
                numEffected += dbs.insert(allUsers);
            }
            if (allGroups.Count > 0)
            {
                numEffected += dbs.insert(allGroups);
            }
            if (allStudents.Count > 0)
            {
                numEffected += dbs.insert(allStudents);
            }
            if (allmeetings.Count > 0)
            {
                numEffected += dbs.insert(allmeetings, m, doc);
            }

            FeedBack_Doc d = dbs.getDoc();
            return d;
            // return numEffected;

        }
        public Dictionary<string,string> ReadWorkbook(string[] path, string p) ///using NOPI 
        {
            IWorkbook book;
            //List<Dictionary<string, string>> report = new List<Dictionary<string, string>>();
            List<Users> allUsers = new List<Users>();
            List<Students> allStudents = new List<Students>();
            List<Groups> allGroups = new List<Groups>();
            List<Group_Meeting> allmeetings = new List<Group_Meeting>();

            Dictionary<string, string> result = new Dictionary<string, string>();

            HebrewCalendar HebCal = new HebrewCalendar();
            //string yearHeb = HebCal.GetYear(DateTime.Now.ToString(string));
            //var currentCulture = CultureInfo.CurrentCulture; // just for reference
            var culture = CultureInfo.CreateSpecificCulture("he-IL"); // proper "Hebrew" culture
            culture.DateTimeFormat.Calendar = HebCal;
            Thread.CurrentThread.CurrentCulture = culture;
            string minYearHeb = HebCal.MinSupportedDateTime.ToString("yyyy"); // min supported year
            string yearHeb = DateTime.Now.ToString("yyyy", culture); // current year
            string heb = yearHeb.Replace("\"", "`");

            FeedBack_Meeting m = new FeedBack_Meeting
            {
                YearMeeting = heb,
                NameMeeting = path[0]

            };

            // m.DetailsCourseDep is null!
            m.DetailsCourseDep = new CoursesAndDepartment();
            m.DetailsCourseDep.NumDepartment = int.Parse(path[1]);
            m.DetailsCourseDep.NumCourse = int.Parse(path[2]);

            var connection = new Dictionary<double, string>();
            //this will help me to find the email for judge/mentor/admin

            FeedBack_Doc doc = new FeedBack_Doc();
            doc.NameDoc = path[0];
            doc.Manager = new List<Users>();

            try
            {
                FileStream fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Try to read workbook as XLSX:
                try
                {
                    book = new XSSFWorkbook(fs);

                    
                    int sheetCount = 0;
                    sheetCount = book.NumberOfSheets;
                  
                    for (int a = 0; a < sheetCount; a++)
                    {

                        ISheet sheet = book.GetSheetAt(a);

                        int colRange = 0;
                        Dictionary<string, int> firstRow = new Dictionary<string, int>();
                        while (sheet.GetRow(0).GetCell(colRange) != null)
                        {
                            firstRow[sheet.GetRow(0).GetCell(colRange).ToString().Trim()] = colRange;
                            colRange++;
                        }

                        string team = "";
                        bool emteyTeam = false;

                        bool oneDate = false;
                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            Students s = new Students();
                            Users u = new Users();
                            u.Type = new Types();
                            Users manager = new Users();
                            manager.Type = new Types();
                            Users jug = new Users();
                            jug.Type = new Types();


                            Groups g = new Groups();
                            g.Mentor = new Users();
                            g.Mentor.Type = new Types();
                            Group_Meeting jg = new Group_Meeting();
                            jg.JudgesGroup = new List<Judge_Group_Meeting>();
                            jg.Group = new Groups();
                            DateTime thisdate = new DateTime();


                            foreach (var j in firstRow)//col
                            {
                                int errRow = row + 1;
                                int errCol = j.Value + 1;

                                switch (j.Key)
                                {
                                    case "צוות מספר":

                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            break;

                                        }
                                        else if(sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            break;
                                        }
                                        else{

                                            
                                            emteyTeam = false;
                                            g.NameGroup = sheet.GetRow(row).GetCell(j.Value).ToString();
                                            team = g.NameGroup;

                                            s.GroupS = new List<Groups>();
                                            Groups gs = new Groups();
                                            gs.NameGroup = team;
                                            s.GroupS.Add(gs);
                                            jg.Group = new Groups();
                                            jg.Group.NameGroup = team;

                                        }
                                        break;

                                    case "שם הסטודנט":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            int numJ = firstRow["מס שופט"];
                                            if ((sheet.GetRow(row).GetCell(j.Value) == null)&&(sheet.GetRow(row).GetCell(numJ).ToString()==null))
                                            {
                                                result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol);
                                                return result;
                                            }
                                        
                                          //  break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        {
                                            string name = sheet.GetRow(row).GetCell(j.Value).ToString();
                                            string newName = stringValidation(name);
                                            s.FirstName = newName;

                                            if (sheet.GetRow(row).GetCell(0) == null)
                                            {
                                                emteyTeam = true;
                                                // s.NumGroup = team;
                                                s.GroupS = new List<Groups>();
                                                Groups gs = new Groups();
                                                gs.NameGroup = team;
                                                s.GroupS.Add(gs);

                                            }
                                            else if (sheet.GetRow(row).GetCell(0).ToString() == "")
                                            {
                                                emteyTeam = true;
                                                // s.NumGroup = team;
                                                s.GroupS = new List<Groups>();
                                                Groups gs = new Groups();
                                                gs.NameGroup = team;
                                                s.GroupS.Add(gs);
                                            }
                                         
                                        }
                                        break;

                                    case "שם משפחה":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol);
                                            return result;
                                            //break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol);
                                            return result;
                                            //break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        {
                                            string name = sheet.GetRow(row).GetCell(j.Value).ToString();
                                            string newName = stringValidation(name);
                                            s.LastName = newName;
                                        }
                                        break;

                                    case "ת.ז":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        {
                                            //s.Id = sheet.GetRow(row).GetCell(j.Value).ToString();
                                           string id = sheet.GetRow(row).GetCell(j.Value).ToString();
                                            if (id.Length != 9)
                                            {
                                                result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol+" תעודת זהות אינה תקינה");
                                                return result;
                                            }
                                            int idnum = int.Parse(id);
                                            bool isNumeric = int.TryParse(id, out idnum);
                                            if (!isNumeric)
                                            {
                                                result.Add("msg", " שגיאה בשורה" + errRow + " ועמודה " + errCol + " תעודת זהות אינה תקינה");
                                                return result;
                                            }

                                        }
                                        break;

                                    case "כתובת מייל":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        {
                                            u.Email = sheet.GetRow(row).GetCell(j.Value).ToString();
                                        }
                                        break;
                                    case "שם משתמש":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        {
                                            u.FirstName = sheet.GetRow(row).GetCell(j.Value).ToString();
                                        }
                                        break;
                                    case "שם משפחה משתמש":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        { 
                                            string name= sheet.GetRow(row).GetCell(j.Value).ToString();
                                            string newName = stringValidation(name);
                                            u.LastName = newName;
                                        }
                                        break;

                                    case "מנהל":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            break;
                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "כן")
                                        {
                                            manager.FirstName = u.FirstName;
                                            manager.LastName = u.LastName;
                                            manager.Email = u.Email;
                                            manager.Type.NumType = 2;
                                            doc.Manager.Add(manager);

                                        }
                                        else
                                        {
                                            break;
                                        }

                                        break;
                                    case "שופט":

                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            break;
                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "כן")
                                        {

                                            jug.FirstName = u.FirstName;
                                            jug.LastName = u.LastName;
                                            jug.Email = u.Email;
                                            jug.Type.NumType = 3;


                                        }
                                        else
                                        {
                                            break;
                                        }

                                        break;
                                    case "מנחה":

                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            break;
                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "כן")
                                        {
                                            u.Type.NumType = 4;

                                        }
                                        else
                                        {
                                            break;
                                        }

                                        break;


                                    case "מס מנחה":
                                        if (emteyTeam == false)
                                        {
                                            var key = Convert.ToDouble(sheet.GetRow(row).GetCell(j.Value).ToString());
                                            //var key = sheet.GetRow(row).GetCell(j.Value);
                                            if (connection.ContainsKey(key))
                                            {
                                                g.Mentor.Email = connection[key];
                                                g.Mentor.Type.NumType = 4;
                                            }


                                        }
                                        break;


                                    case "מס שופט":
                                        if (emteyTeam == false)
                                        {

                                            Judge_Group_Meeting ju = new Judge_Group_Meeting();
                                            ju.Judge = new Users();
                                            ju.Judge.Type = new Types();
                                            var key = Convert.ToDouble(sheet.GetRow(row).GetCell(j.Value).ToString());
                                            //var key = sheet.GetRow(row).GetCell(j.Value); ///???////
                                            if (connection.ContainsKey(key))
                                            {
                                                ju.Judge.Email = connection[key];
                                                ju.Judge.Type.NumType = 3;
                                                jg.JudgesGroup.Add(ju);
                                                int coun = 1;
                                                if(coun + row > sheet.LastRowNum)
                                                {
                                                    break;
                                                }
                                                else if(sheet.GetRow(row + coun).GetCell(j.Value) == null)
                                                {
                                                    break;
                                                }
                                                while ((sheet.GetRow(row + coun).GetCell(j.Value).ToString() != ""))
                                                {
                                                    //xlRange.Cells[i + coun, j].Value2                         //  xlRange.Cells[i + coun, 1].Value2 

                                                    if (sheet.GetRow(row + coun).GetCell(j.Value).ToString() != "" && sheet.GetRow(row + coun).GetCell(0).ToString() == "")
                                                    {
                                                        var key1 = Convert.ToDouble(sheet.GetRow(row+coun).GetCell(j.Value).ToString());
                                                        if (connection.ContainsKey(key1))
                                                        {
                                                            ju.Judge.Email = connection[key1];
                                                            ju.Judge.Type.NumType = 3;
                                                            jg.JudgesGroup.Add(ju);
                                                        }

                                                    }
                                                    else if (sheet.GetRow(row + coun).GetCell(j.Value).ToString() != "" && sheet.GetRow(row + coun).GetCell(0)==null)
                                                    {
                                                        var key1 = Convert.ToDouble(sheet.GetRow(row + coun).GetCell(j.Value).ToString());
                                                        if (connection.ContainsKey(key1))
                                                        {
                                                            ju.Judge.Email = connection[key1];
                                                            ju.Judge.Type.NumType = 3;
                                                            jg.JudgesGroup.Add(ju);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                    coun++;
                                                    if(coun+row> sheet.LastRowNum)//if I in the last row..
                                                    {
                                                        break;
                                                    }
                                                }
                                            }

                                        }
                                        break;

                                    case "שם הפרויקט":
                                        if (emteyTeam == false)
                                        {
                                            string name = sheet.GetRow(row).GetCell(j.Value).ToString();
                                            string newName = stringValidation(name);
                                            //string newName = name.Replace("\"", "`");
                                            g.NameProject = newName;


                                        }
                                        break;

                                    case "ארגון":
                                        if (emteyTeam == false)
                                        {
                                            string name = sheet.GetRow(row).GetCell(j.Value).ToString();
                                            string newName = stringValidation(name);
                                            //string newName = name.Replace("\"", "`");
                                            g.NameOrganization = newName;
                                        }
                                        break;

                                    case "סוג הפרויקט":
                                        if (emteyTeam == false)
                                        {
                                            g.ProjectType = sheet.GetRow(row).GetCell(j.Value).ToString();
                                        }
                                        break;

                                    case "שעת התחלה":
                                        if (emteyTeam == false)
                                        {//??????????????????//////
                                            jg.StartTime = Convert.ToDateTime(sheet.GetRow(row).GetCell(j.Value).ToString()).ToString("HH:mm");
                                            // jg.StartTime = DateTime.FromOADate(sheet.GetRow(row).GetCell(j.Value).ToString()).ToString("HH:mm");
                                            // jg.StartTime = (DateTime)xlRange.Cells[i, j].Value2;
                                        }
                                        break;
                                    case "שעת סיום":
                                        if (emteyTeam == false)
                                        {
                                            jg.EndTime = Convert.ToDateTime(sheet.GetRow(row).GetCell(j.Value).ToString()).ToString("HH:mm");
                                            // jg.EndTime = DateTime.FromOADate(Convert.ToDouble(sheet.GetRow(row).GetCell(j.Value).ToString())).ToString("HH:mm");
                                        }
                                        break;
                                    case "מס רץ":
                                        if (sheet.GetRow(row).GetCell(j.Value) == null)
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                        {
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                       else if (sheet.GetRow(row).GetCell(j.Value+1) == null)
                                        {
                                            int errCol2 = errCol + 1;
                                            result.Add("msg", " שגיאה בשורה" + errRow + " ובעמודה " + errCol2);
                                            return result;
                                           // break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                        }
                                        else if (sheet.GetRow(row).GetCell(j.Value+1).ToString() == "")
                                        {
                                            int errCol2 = errCol + 1;
                                            result.Add("msg", " שגיאה בשורה" + errRow + " בעמודה " + errCol2);
                                            return result;
                                          //  break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                        }
                                        else
                                        {
                                            double x = Convert.ToDouble(sheet.GetRow(row).GetCell(j.Value).ToString());
                                            
                                                string y = sheet.GetRow(row).GetCell(j.Value + 1).ToString();
                                            if (!IsValid(y))
                                            {
                                                int errCol2 = errCol + 1;
                                                result.Add("msg", " שגיאה בשורה" + errRow + " בעמודה " + errCol2+" כתובת אימייל אינה תקינה");
                                                return result;
                                            }
                                            connection.Add(x, y);
                                        }

                                        break;
                                    case "תאריך":
                                        if (oneDate == false)
                                        {
                                            if (sheet.GetRow(row).GetCell(j.Value) == null)
                                            {

                                                break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה

                                            }
                                            else if (sheet.GetRow(row).GetCell(j.Value).ToString() == "")
                                            {
                                                break;//החזרת שגיאה שמשהו לא תקין ולכן הקובץ אקסל לא הועלה
                                            }
                                            else
                                            {
                                                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-IL");
                                                DateTime date = Convert.ToDateTime(sheet.GetRow(row).GetCell(j.Value).ToString());
                                                // DateTime date = DateTime.FromOADate(Convert.ToDouble(sheet.GetRow(row).GetCell(j.Value).ToString()));
                                                //  date.ToString("dd/MM/yyyy");

                                                m.Date = date;
                                                thisdate = date;
                                                oneDate = true;
                                            }

                                        }
                                        else
                                        {
                                            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-IL");
                                            DateTime date1 = Convert.ToDateTime(sheet.GetRow(1).GetCell(j.Value).ToString());
                                            m.Date = date1;
                                        }
                                        break;
                                }
                            }
                            //ברגע שסיימתי לעבור על שורה
                            if (s.Id != null)
                            {
                                allStudents.Add(s);
                            }
                            if (u.Type.NumType != 0)
                            {
                                allUsers.Add(u);
                            }
                            if (jug.Email != null)
                            {
                                allUsers.Add(jug);
                            }
                            if (manager.Email != null)
                            {
                                allUsers.Add(manager);
                            }
                            if (g.NameGroup != null)
                            {
                                allGroups.Add(g);
                            }
                            if (jg.Group.NameGroup != null)
                            {
                                allmeetings.Add(jg);
                            }


                        }
                    }




                }
                catch (Exception ex)
                {
                    throw (ex);
                    book = null;
                }

                // If reading fails, try to read workbook as XLS:
                if (book == null)
                {
                    book = new HSSFWorkbook(fs);
                }
            }
            catch (Exception ex)
            {

                this.Close();

            }

            int numEffected = 0;
            DBservices dbs = new DBservices();
            if (allUsers.Count > 0)
            {
                numEffected += dbs.insert(allUsers);
            }
            if (allGroups.Count > 0)
            {
                numEffected += dbs.insert(allGroups);
            }
            if (allStudents.Count > 0)
            {
                numEffected += dbs.insert(allStudents);
            }
            if (allmeetings.Count > 0)
            {
                numEffected += dbs.insert(allmeetings, m, doc);
            }

            FeedBack_Doc d = dbs.getDoc();
          //  List<Dictionary<string, string>> msg = new List<Dictionary<string, string>>();

            result.Add("id", d.NumDoc.ToString());
            return result;

        }
        private void Close()
        {
            {
                throw new NotImplementedException();
            }

        }

        private string stringValidation(string name)
        {
            string name1=name;
            if (name.Contains("\""))
            {
                name1 = name.Replace("\"", "`");
            }
            if (name.Contains("'" ) && name.Contains("\""))
            {
                 name1 = name1.Replace("\"", "`");
            }
            else if(name.Contains("'"))
            {
                name1 = name.Replace("\"", "`");
            }

            return name1;

        }
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
       
    }
   

}

