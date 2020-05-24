using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace mashovFinal.Utilities
{
    public class StringHelper
    {
        private static Random ran = new Random();

        public static string GeneratePassword()
        {

            string pass = "";
            pass = Membership.GeneratePassword(8, 0) + ran.Next(5, 10);
            pass = Regex.Replace(pass, @"[^a-zA-Z0-9]", m => ran.Next(0, 10).ToString());//להגדיר מתחלתחילה בלי תווים מיוחדים 

            return pass;
        }
    }
}