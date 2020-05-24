using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;

namespace mashovFinal.Controllers
{
    public class CalendarController : ApiController
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
        
    }
}
