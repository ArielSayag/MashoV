using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mashovFinal.Models
{
    public class Judge_Group_Meeting
    {
        Users judge;
        double sumScore;
        //List<Group_Meeting>

      
        public Users Judge { get => judge; set => judge = value; }
        public double SumScore { get => sumScore; set => sumScore = value; }



        public Judge_Group_Meeting() { }
        public Judge_Group_Meeting(Users id,string n, double score)
        {
            judge = id;
          //  nameJudge = n;
            sumScore = score;
         
         
        }
    }
}