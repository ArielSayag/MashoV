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
        DateTime complet;

        public int Id { get => id; set => id = value; }
        public Users Mentor { get => mentor; set => mentor = value; }
        public Groups Group { get => group; set => group = value; }
        public Students Student { get => student; set => student = value; }
        public string NameTask { get => nameTask; set => nameTask = value; }
        public DateTime Complet { get => complet; set => complet = value; }

        public calendarTask() {}
        public calendarTask (int i,Users u,Groups g,Students s,string n,DateTime c)
        {
            id = i;
            mentor = u;
            group = g;
            student = s;
            nameTask = n;
            complet = c;
        }
        public int insert()
        {
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
    }
}