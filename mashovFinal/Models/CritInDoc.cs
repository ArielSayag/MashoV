using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class CritInDoc
    {
        List<Criterion> allCrit;
        FeedBack_Doc doc;

        public List<Criterion>AllCrit { get => allCrit; set => allCrit = value; }
        public FeedBack_Doc Doc { get => doc; set => doc = value; }

        public CritInDoc() { }
        public CritInDoc(List<Criterion> c,FeedBack_Doc d)
        {
            allCrit = c;
            doc = d;
        }
        public int insert()
        {
            DBservices dbs = new DBservices();
            return dbs.insert(this);
        }
        public int updateAndinsertCrit()
        {
            DBservices dbs = new DBservices();
            return dbs.insAndUpCrit(this);
        }

    }
}