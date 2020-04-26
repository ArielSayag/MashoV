using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;

namespace mashovFinal.Controllers
{
    public class UserController : ApiController
    {
        [HttpPut]
        [Route("api/User")]
        public List<FeedBack_Doc> Put([FromBody]Users u)
        {

            return u.getUserDetails();
        }
        //[HttpGet]
        //[Route("/api/User/judge/{getuser}")]
        //public List<FeedBack_Meeting> GetJudge([FromBody]Users u)
        //{

        //    return u.getJudge();
        //}

    }
}
