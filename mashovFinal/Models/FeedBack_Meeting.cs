using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace mashovFinal.Models
{
    public class FeedBack_Meeting
    {
        int numMeeting;
        int numDoc;
        string nameMeeting;
        string yearMeeting;
        string date;
        CoursesAndDepartment detailsCourseDep;

        public int NumMeeting { get => numMeeting; set => numMeeting = value; }
        public int NumDoc { get => numDoc; set => numDoc = value; }
        public string NameMeeting { get => nameMeeting; set => nameMeeting = value; }
        public string YearMeeting { get => yearMeeting; set => yearMeeting = value; }
        public string Date { get => date; set => date = value; }
        public CoursesAndDepartment DetailsCourseDep { get => detailsCourseDep; set => detailsCourseDep = value; }

        public FeedBack_Meeting() { }
        public FeedBack_Meeting(int n, string name,string year, string dateDay, CoursesAndDepartment candD,int nd)
        {
            numMeeting = n;
            nameMeeting = name;
            yearMeeting = year;
            date = dateDay;
            detailsCourseDep = candD;
            numDoc = nd;
        
        }


        //        CREATE TABLE Judge_Groups(
        //IDJudge nvarchar(9) NOT NULL,
        //NumGroup nvarchar(9) NOT NULL,
        //utID int NOT NULL,
        //sumScore float,

        //PRIMARY KEY(IDJudge, NumGroup),
        //FOREIGN KEY(IDJudge, utID) REFERENCES Users(ID, utID),
        //FOREIGN KEY(NumGroup) REFERENCES Groups(NumGroup),
        //);

        //CREATE TABLE FeedBack_Session_Groups(
        //NumMeeting int IDENTITY (1,1) NOT NULL,
        //NumGroup nvarchar(9) NOT NULL,
        //TimeMeeting Time(0) NOT NULL,

        //FOREIGN KEY(NumMeeting) REFERENCES FeedBack_Session(NumMeeting),
        //FOREIGN KEY(NumGroup) REFERENCES Groups(NumGroup),
        //);
        //        CREATE TABLE FeedBack_Session(
        //NumMeeting int IDENTITY (1,1) NOT NULL,
        //NameMeeting nvarchar(90) NOT NULL,
        //DateMeeting Date NOT NULL , 
        //TimeMeeting Time(0) NOT NULL,?
        //yearMeeting nvarchar(10) NOT NULL,
        //NumCourse int NOT NULL,
        //NumDoc int NOT NULL,

        //PRIMARY KEY(NumMeeting),
        //FOREIGN KEY(NumCourse) REFERENCES Course(NumCourse),
        //FOREIGN KEY(NumDoc) REFERENCES FeedBack_Doc(NumDoc),
        //);
    }
}