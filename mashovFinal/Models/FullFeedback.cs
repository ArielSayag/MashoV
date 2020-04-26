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
        

        public CritInDoc CritDoc { get => critDoc; set => critDoc = value; }
        public Group_Meeting GroupM { get => groupM; set => groupM = value; }

        public FullFeedback() { }
        public FullFeedback(CritInDoc cd, Group_Meeting gm)
        {
            critDoc = cd;
            groupM = gm;
        }
    }
}