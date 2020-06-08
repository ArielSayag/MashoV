using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class calendarTask
    {
        int id;
        Users mentor;
        Groups group;
        Students student;
        string nameTask;
        string complet;
        string startdate;
        string desc;
        int status;

        public int Id { get => id; set => id = value; }
        public Users Mentor { get => mentor; set => mentor = value; }
        public Groups Group { get => group; set => group = value; }
        public Students Student { get => student; set => student = value; }
        public string NameTask { get => nameTask; set => nameTask = value; }
        public string Complet { get => complet; set => complet = value; }
        public string Startdate { get => startdate; set => startdate = value; }
        public string Desc { get => desc; set => desc = value; }
        public int Status { get => status; set => status = value; }

        public calendarTask() {}
        public calendarTask (int i,Users u,Groups g,Students s,string n,string c,string st,string d ,int s1)
        {
            id = i;
            mentor = u;
            group = g;
            student = s;
            nameTask = n;
            complet = c;
            startdate = st;
            desc = d;
            status = s1;
        }
        public int insert()
        {
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
        public int updateT()
        {
            DBservices dbs = new DBservices();
            return dbs.updateT(this);
        }
        public int deleteT(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.deleteT(id);
        }
        public List<calendarTask> getallcalendarTByStudent(string id)
        {
            DBservices dbs = new DBservices();
            return dbs.GetCalendarTaskStudent(id);
        }

    }
}