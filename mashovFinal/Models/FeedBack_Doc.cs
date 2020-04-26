using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class FeedBack_Doc
    {
        int numDoc;
        string nameDoc;
        FeedBack_Meeting detailsMeet;
        List<Users> manager;
        bool status;
        double totalWeight;

        public int NumDoc { get => numDoc; set => numDoc = value; }
        public string NameDoc { get => nameDoc; set => nameDoc = value; }
        public FeedBack_Meeting DetailsMeet { get => detailsMeet; set => detailsMeet = value; }
        public List<Users> Manager { get => manager; set => manager = value; }
        public bool Status { get => status; set => status = value; }
        public double TotalWeight { get => totalWeight; set => totalWeight = value; }
        public FeedBack_Doc() { }
        public FeedBack_Doc(int num, string name, FeedBack_Meeting meet, List<Users> m, bool b,double total)
        {
            numDoc = num;
            nameDoc = name;
            detailsMeet = meet;
            manager = m;
            status = b;
            totalWeight = total;

        }

        public CritInDoc getCrits()
        {
            DBservices dbs = new DBservices();

            return dbs.get(this);
        }
        public FeedBack_Doc getDoc()
        {
            DBservices dbs = new DBservices();

            return dbs.getDoc();
        }
    }
}