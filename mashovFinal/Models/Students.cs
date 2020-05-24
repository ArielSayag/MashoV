using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Students
    {
        List<Groups> groupS;


        string id;
        string firstName;
        string lastName;
        string email;
        string pass;
        double finalScore;

        public string Id { get => id; set => id = value; }
        public List<Groups> GroupS { get => groupS; set => groupS = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Email { get => email; set => email = value; }
        public string Pass { get => pass; set => pass = value; }
        public double FinalScore { get => finalScore; set => finalScore = value; }


        public Students() { }
        public Students(string i, List<Groups> num, string first, string last, string e, string p, double f)
        {
            pass = p;
            id = i;
            groupS = num;
            firstName = first;
            lastName = last;
            email = e;
            finalScore = f;

        }

        public Students checkingExist()
        {
            DBservices dbs = new DBservices();

            return dbs.checkingExist(this);
        }
        public int update()
        {
            DBservices dbs = new DBservices();

            return dbs.updetpass(this);
        }
        public List<Students> getAllstudents(int idDoc)
        {
            DBservices dbs = new DBservices();
            return dbs.getStudent(idDoc);
        }
        public int updateGroupsOfStudent()
        {
            DBservices dbs = new DBservices();
            return dbs.updateStudentInGroup(this);
        }
        public List<Group_Meeting> getStudentDoc(string id)
        {
            DBservices dbs = new DBservices();
            return dbs.getStudentDocs(id);
        }
        public List<Group_Meeting> getStudentDocMeet(string id, Group_Meeting g)
        {
            DBservices dbs = new DBservices();
            return dbs.getStudentDocsMeet(id, g);
        }
        public List<Students> getFinalScore(int id)
        {
            DBservices dbs = new DBservices();
            return dbs.getFinalScore(id);
        }
        public Students updateEmail()
        {
            DBservices dbs = new DBservices();
            return dbs.updateEmailStudent(this);
        }
        public int updateStudent()
        {
            DBservices dbs = new DBservices();
            return dbs.updateStudentDetails(this);
        }
    }
}