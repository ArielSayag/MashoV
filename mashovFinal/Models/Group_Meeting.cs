using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Group_Meeting
    {
        
        int numMeeting;
        Groups group;
        string startTime;
        string endTime;
       List<Judge_Group_Meeting> judgesGroup;


        public int NumMeeting { get => numMeeting; set => numMeeting = value; }
        public Groups Group { get => group; set => group = value; }
        public string StartTime { get => startTime; set => startTime = value; }
        public string EndTime { get => endTime; set => endTime = value; }
        public List<Judge_Group_Meeting> JudgesGroup { get => judgesGroup; set => judgesGroup = value; }

        public Group_Meeting() { }
        public Group_Meeting(int n, Groups numG,  string time, string end , List<Judge_Group_Meeting> j)
        {
            numMeeting = n;
            group = numG;
            startTime = time;
            endTime = end;
            judgesGroup = j;
        }
        public int insertjudgeInGroup()
        {
           
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
    }
}