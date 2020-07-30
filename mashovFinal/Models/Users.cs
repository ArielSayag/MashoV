using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace mashovFinal.Models
{
    public class Users
    {
        string pass;
        string firstName;
        string lastName;
        string email;
        Types type;
        List<Types> typesofuser;
        string updateemail;
        int numDoc; //for judae_index3

        public string Pass { get => pass; set => pass = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Email { get => email; set => email = value; }
        public Types Type { get => type; set => type = value; }
        public List<Types> Typesofuser { get => typesofuser; set => typesofuser = value; }
        public string Updateemail { get => updateemail; set => updateemail = value; }
        public int NumDoc { get => numDoc; set => numDoc = value; }



        public Users() { }
        public Users( string p,string first, string last, string em, Types ty ,List<Types> list,string ue,int n)
        {
            pass = p;
            firstName = first;
            lastName = last;
            email = em;
            type = ty;
            typesofuser = list;
            updateemail = ue;
            numDoc = n;
          //  numGroup = ng;
        }
        public Users ifexist()
        {
            DBservices dbs = new DBservices();
            
            return dbs.checkingExistUser(this);
        }
        public int update()
        {
            DBservices dbs = new DBservices();

            return dbs.updetpass(this);
        }

        public List<FeedBack_Doc> getUserDetails()
        {
            DBservices dbs = new DBservices();

            return dbs.get(this);
        }
        public List<Users> getAllusers()
        {
            DBservices dbs = new DBservices();
            return dbs.getUsers();
        }
        public int insertNewUser()
        {
           
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
        public List<FeedBack_Meeting> getJudge()
        {
            DBservices dbs = new DBservices();
            return dbs.getJudgeCourse(this);
        }
        public List<Group_Meeting> getJudgeGroup(int index)
        {
            DBservices dbs = new DBservices();
            return dbs.getJudgeGroups(this,index);
        }
        public List<FeedBack_Meeting> getMentor()
        {
            DBservices dbs = new DBservices();
            return dbs.getMentorCourse(this);
        }
        public List<Group_Meeting> getMentorGroup(int index)
        {
            DBservices dbs = new DBservices();
            return dbs.getMentorGroups(this, index);
        }
        public int updateUsers()
        {
            DBservices dbs = new DBservices();
            return dbs.updateUserDetails(this);
        }

        public List<calendarMeeting> getallcalendarMeetingByMentor()
        {
            DBservices dbs = new DBservices();
            return dbs.GetCalendarMeetings(this);
        }
        public List<calendarTask> getcalendarTaskbymentor()
        {
            DBservices dbs = new DBservices();
            return dbs.GetCalendarTask(this);
        }
        public List<Groups> getcalG()
        {
            DBservices dbs = new DBservices();
            return dbs.getmentorG(this);
        }
        public List<Users> getAllJ(int num)
        {
            DBservices dbs = new DBservices();
            return dbs.getAllJ( num);
        }
      
    }
}