using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;

namespace mashovFinal.Controllers
{
    public class StudentController : ApiController
    {
        [HttpGet]
        [Route("api/Student/{studentID}")]
        public List<Group_Meeting> Get(string studentID)
        {
            Students s = new Students();
            return s.getStudentDoc(studentID);
        }
        [HttpPut]
        [Route("api/Student/{id}")]
        public List<Group_Meeting> Put(string id,[FromBody] Group_Meeting g)
        {
            Students s = new Students();
            return s.getStudentDocMeet(id,g);
        }
        [HttpPost]
        [Route("api/Student")]
        public List<Students> Post([FromBody] FeedBack_Doc d)
        {
            Students s = new Students();

             int index = d.NumDoc;
           
            return s.getFinalScore(index);
        }
    }
   
}

