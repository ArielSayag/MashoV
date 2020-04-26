using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Courses
    {
        int numCourse;
        string nameCourse;

        public int NumCourse { get => numCourse; set => numCourse = value; }
        public string NameCourse { get => nameCourse; set => nameCourse = value; }


        public Courses() { }
        public Courses(int num, string name)
        {
            numCourse = num;
            nameCourse = name;
        }

        public List<Courses> getCourses()
        {
            DBservices dbs = new DBservices();
            return dbs.getAllCourses();
        }
        public int insert(string c)
        {
            DBservices dbs = new DBservices();
            return dbs.insert(c,null);
        }
    }
}