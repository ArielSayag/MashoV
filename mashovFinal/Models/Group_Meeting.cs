using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Group_Meeting
    {
        
        int numMeeting;
        FeedBack_Meeting feedBackMeet;
        Groups group;
        string startTime;
        string endTime;
       List<Judge_Group_Meeting> judgesGroup;
        double sum;

        public int NumMeeting { get => numMeeting; set => numMeeting = value; }
        public FeedBack_Meeting FeedBackMeet { get => feedBackMeet; set => feedBackMeet = value; }
        public Groups Group { get => group; set => group = value; }
        public string StartTime { get => startTime; set => startTime = value; }
        public string EndTime { get => endTime; set => endTime = value; }
        public List<Judge_Group_Meeting> JudgesGroup { get => judgesGroup; set => judgesGroup = value; }
        public double Sum { get => sum; set => sum = value; }

        public Group_Meeting() { }
        public Group_Meeting(int n, Groups numG,  string time, string end , List<Judge_Group_Meeting> j,double f,FeedBack_Meeting fm)
        {
            numMeeting = n;
            group = numG;
            startTime = time;
            endTime = end;
            judgesGroup = j;
            sum = f;
            feedBackMeet = fm;
        }
        public int insertjudgeInGroup()
        {
           
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
    }
}