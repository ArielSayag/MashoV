using System;
using System.Collections.Generic;
using System.Data;
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
            List<Criterion> critafterinsert= dbs.insert(this);
            dbs = dbs.readFromDB();
            dbs.dt= updetFevorite(critafterinsert, dbs.dt);
            dbs.update();
            return 0;
           
            
        }
        
        private DataTable updetFevorite(List<Criterion> c, DataTable dt)
        {
            
            bool flag = true;
            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["NumCrit"]);
                foreach (var i in c)
                {   
                    if (id == i.NumCrit)
                    {
                        flag = false;
                        dr["counterScore"] = Convert.ToInt32(dr["counterScore"]) + 1;
                        break;
                    }
                }
               
            }
            //if (flag)
            //{
            //    DBservices dbs = new DBservices();
            //    dbs.insert(f);
            //}
            return dt;

        }


        public int updateDocLastDoc()
        {
            DBservices dbs = new DBservices();
             int num=dbs.updateDocwithLastDoc(this);
           // List<Criterion> critafterinsert = dbs.insert(this);
            dbs = dbs.readFromDB();
            dbs.dt = updetFevorite(this.allCrit, dbs.dt);
            dbs.update();
            return num;
        }
        public int updateAndinsertCrit()
        {
            DBservices dbs = new DBservices();
            return dbs.insAndUpCrit(this);
        }
        

        public CritInDoc getTest(int numMeet)
        {
            DBservices dbs = new DBservices();
            return dbs.getMeetDoc(numMeet);
        }
        public CritInDoc getTestupdate(int numGroup, Users judge)
        {
            DBservices dbs = new DBservices();
            return dbs.getMeetDocupdate(numGroup, judge);
        }
        public List<CritInDoc> getLast(CoursesAndDepartment cd)
        {
            DBservices dbs = new DBservices();
            return dbs.lastDoc(cd);
        }
        public int deleteCritFromDoc()
        {
            DBservices dbs = new DBservices();
            return dbs.delete(this);
        }
    }
}