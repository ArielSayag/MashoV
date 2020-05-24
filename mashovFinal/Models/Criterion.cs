using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Criterion
    {
        int numCrit;
        string nameCrit;
        string descriptionCrit;
        double weightCrit;
        int typeCrit; //numScala
        double score;
        string note;
        string nameScala;
        string typeCssName; // only for the function SaveToData index3.html
        string valueCrit;
        int favorite;


        public int NumCrit { get => numCrit; set => numCrit = value; }
        public string NameCrit { get => nameCrit; set => nameCrit = value; }
        public string DescriptionCrit { get => descriptionCrit; set => descriptionCrit = value; }
        public double WeightCrit { get => weightCrit; set => weightCrit = value; }
        public int TypeCrit { get => typeCrit; set => typeCrit = value; }
        public string NameScala { get => nameScala; set => nameScala = value; }
        public double Score { get => score; set => score = value; }
        public string Note { get => note; set => note = value; }
        public string TypeCssName { get => typeCssName; set => typeCssName = value; }
        public string ValueCrit { get => valueCrit; set => valueCrit = value; }
        public int Favorite { get => favorite; set => favorite = value; }

        public Criterion() { }
        public Criterion(int num , string name, string des, double we, int ty, double sc, string no ,string names,string t, string v,int f)
        {
            numCrit = num;
            nameCrit = name;
            descriptionCrit = des;
            weightCrit = we;
            typeCrit = ty;
            score = sc;
            note = no;
            nameScala = names;
            TypeCssName = t;
            valueCrit = v;
            favorite = f;

        }

        public List<Criterion> getAllCrit()
        {
            DBservices dbs = new DBservices();
            return dbs.getAllCrit();
        }



    }
}