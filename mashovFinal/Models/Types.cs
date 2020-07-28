using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace mashovFinal.Models
{
    public class Types
    {
        int numType;
        string type;

        public int NumType { get => numType; set => numType = value; }
        public string Type { get => type; set => type = value; }

        public Types() { }
        public Types(int num , string t)
        {
            numType = num;
            type = t;
        }
        //public List<Types> getAlltypes()
        //{
        //    DBservices dbs = new DBservices();
        //    return dbs.getType();
        //}
    }
}