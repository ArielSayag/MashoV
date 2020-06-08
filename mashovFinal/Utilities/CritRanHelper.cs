using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mashovFinal.Models;

namespace mashovFinal.Utilities
{
    public class CritRanHelper
    {
        private static Random ran = new Random();

        public static void checkDocs()
        {
            DBservices dbs = new DBservices();
            int status=dbs.checkedDoc();
       
            
            
        }

        public static List<Criterion> createNewListCrit(List<Criterion> dataList,double total)
        {
           
            List<Criterion> newc = new List<Criterion>();
            
            Dictionary<int, List<Criterion>> best = new Dictionary<int, List<Criterion>>();
            List<Criterion> win = new List<Criterion>();
            double total1 = total;
            int score=0;
            foreach (var item in dataList)
            {
                var difference = 100 - total;
                if ((item.WeightCrit <= difference) &&(total<100))
                {
                    newc.Add(item);
                    total += item.WeightCrit;
                    score += item.Favorite;
                    
                }
                else if (total == 100)
                {
                    best.Add(score, newc);          
                     newc = new List<Criterion>();
                    total = total1;
                    score = 0;
                }
            }

            List<Criterion> selected = new List<Criterion>();
            int  score1 = 0;
            foreach (var item in best)
            {
                if (item.Key > score1)
                {
                    score1 = item.Key;
                    win = item.Value;
                }
            }
          

                return win;
        }
    }
}