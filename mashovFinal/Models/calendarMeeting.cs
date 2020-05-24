using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class calendarMeeting
    {
        int id;
        string startTime;
        string endTime;
        string dateSelected;
        Users mentor;
        Groups group;
        string placeMetting;

        public int Id { get => id; set => id = value; }
        public string StartTime { get => startTime; set => startTime = value; }
        public string EndTime { get => endTime; set => endTime = value; }
        public string DateSelected { get => dateSelected; set => dateSelected = value; }
        public Users Mentor { get => mentor; set => mentor = value; }
        public Groups Group { get => group; set => group = value; }
        public string PlaceMetting { get => placeMetting; set => placeMetting = value; }

        public calendarMeeting() { }
        public calendarMeeting(int i, string s, string e, string d, Users u,Groups g,string p)
        {
            id = i;
            startTime = s;
            endTime = e;
            dateSelected = d;
            mentor = u;
            group = g;
            placeMetting = p;
        }
        public int insert()
        {
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
      
    }
}