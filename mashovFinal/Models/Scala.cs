using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Scala
    {
        int numScala;
        string nameScala;

        public int NumScala { get => numScala; set => numScala = value; }
        public string NameScala { get => nameScala; set => nameScala = value; }

        public Scala() { }
        public Scala(int num, string t)
        {
            numScala = num;
            nameScala = t;
        }

        public List<Scala> getScala()
        {
            DBservices dbs = new DBservices();
            return dbs.getListScala();
        }
    }
}