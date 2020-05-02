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
        [HttpPut]
        [Route("api/User/judge")]
        public List<FeedBack_Meeting> PutJudge([FromBody]Users u)
        {

            return u.getJudge();
        }
        [HttpPut]
        [Route("api/User/judge/Groups/{metting}")]
        public List<Group_Meeting> Put(int metting, [FromBody]Users u)
        {

            return u.getJudgeGroup(metting);
        }
    }
}
