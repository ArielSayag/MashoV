using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;

namespace mashovFinal.Controllers
{
    public class calendarController : ApiController
    {
        [HttpPost]
        [Route("api/calendar/Meet")]
        public List<calendarMeeting> PostMeet([FromBody] Users mentor)
        {

            return mentor.getallcalendarMeetingByMentor();
        }
        [HttpPost]
        [Route("api/calendar")]
        public List<calendarTask> Post([FromBody] Users mentor)
        {

            return mentor.getcalendarTaskbymentor();
        }
        [HttpPost]
        [Route("api/calendar/insertM")]
        public int PostinsertM([FromBody] calendarMeeting cm)
        {

            return cm.insert();
        }
        [HttpPost]
        [Route("api/calendar/insertT")]
        public int PostinsertM([FromBody] calendarTask ct)
        {

            return ct.insert();
        }
        [HttpPut]
        [Route("api/calendar/Group")]
        public List<Groups> Put([FromBody] Users u)
        {

            return u.getcalG();
        }
        [HttpPut]
        [Route("api/calendar/updateM")]
        public int PutupdateM([FromBody] calendarMeeting m)
        {

            return m.updateM();
        }
        [HttpPut]
        [Route("api/calendar/updateT")]
        public int PutupdateT([FromBody] calendarTask t)
        {

            return t.updateT();
        }

        [HttpGet]
        [Route("api/calendar/deleteM/{idm}")]
        public int getdaleteM( int idm)
        {
            calendarMeeting m = new calendarMeeting();
            return m.deleteM(idm);
        }

        [HttpPut]
        [Route("api/calendar/deleteT")]
        public int PutdeleteT([FromBody] int id)
        {
            calendarTask t = new calendarTask();
            return t.deleteT(id);
        }

        //----------student------------------//

        [HttpGet]
        [Route("api/calendar/Meet/{idStudent}")]
        public List<calendarMeeting> GetMeet(string idStudent)
        {
            calendarMeeting m = new calendarMeeting();
            return m.getallcalendarMeetingByStudent(idStudent);
        }

        [HttpGet]
        [Route("api/calendar/Task/{idStudent}")]
        public List<calendarTask> GetTask( string idStudent)
        {
            calendarTask t = new calendarTask();
            return t.getallcalendarTByStudent(idStudent);
        }

        [HttpGet]
        [Route("api/calendar/Mentor/{idStudent}")]
        public List<Users> GetMentor(string idStudent)
        {
            Students s = new Students();
            return s.getallmentorbystudent(idStudent);
        }


        [HttpPut]
        [Route("api/calendar/StatusT/")]
        public int PutStatusT([FromBody]int id)
        {
            Students s = new Students();
            return s.updateStatusT(id);
        }

        [HttpGet]
        [Route("api/calendar/Groups/{id}")]
        public List<Groups> gatGroups(string id)
        {
            Students s = new Students();
            return s.getGroupsMentorofS(id);
        }
    }
}
