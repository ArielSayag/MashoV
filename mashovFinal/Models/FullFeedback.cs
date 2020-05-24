using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class FullFeedback
    {
        CritInDoc critDoc;
        Group_Meeting groupM;
        bool statusFull;
        float sum;
        Users judes;

        public CritInDoc CritDoc { get => critDoc; set => critDoc = value; }
        public Group_Meeting GroupM { get => groupM; set => groupM = value; }
        public bool StatusFull { get => statusFull; set => statusFull = value; }
        public float Sum { get => sum; set => sum = value; }
        public Users Judes { get => judes; set => judes = value; }

        public FullFeedback() { }
        public FullFeedback(CritInDoc cd, Group_Meeting gm,bool s,float f,Users j)
        {
            critDoc = cd;
            groupM = gm;
            statusFull = s;
            sum = f;
            judes = j;
        }

        public int insert()
        {
            DBservices dbs = new DBservices();
           return dbs.insert(this);
        }
        public int update()
        {
            DBservices dbs = new DBservices();
            return dbs.update(this);
        }
    }
}