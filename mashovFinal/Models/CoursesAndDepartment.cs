using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class CoursesAndDepartment
    {
        int numDepartment;
        string nameDepartment;
        int numCourse;
        string nameCourse;

        public int NumDepartment { get => numDepartment; set => numDepartment = value; }
        public string NameDepartment { get => nameDepartment; set => nameDepartment = value; }
        public int NumCourse { get => numCourse; set => numCourse = value; }
        public string NameCourse { get => nameCourse; set => nameCourse = value; }



        public CoursesAndDepartment() { }
        public CoursesAndDepartment(int numD, string nameD, int numC, string nameC)
        {
            numDepartment = numD;
            nameDepartment = nameD;
            numCourse = numC;
            nameCourse = nameC;

        }
        //public int insert(CoursesAndDepartment coursDep)
        //{
        //    DBservices dbs = new DBservices();
        //    int numEffected = dbs.insert(coursDep);
        //    return numEffected;


        //}


    }
}