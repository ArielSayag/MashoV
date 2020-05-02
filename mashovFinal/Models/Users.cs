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

        public string Pass { get => pass; set => pass = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Email { get => email; set => email = value; }
        public Types Type { get => type; set => type = value; }
        public List<Types> Typesofuser { get => typesofuser; set => typesofuser = value; }



        public Users() { }
        public Users( string p,string first, string last, string em, Types ty ,List<Types> list)
        {
            pass = p;
            firstName = first;
            lastName = last;
            email = em;
            type = ty;
            typesofuser = list;
          //  numGroup = ng;
        }
        public List<Users> ifexist()
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
            List<Users> newuser = new List<Users>();
            newuser.Add(this);
            DBservices dbs = new DBservices();
            return dbs.insert(newuser);
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
    }
}