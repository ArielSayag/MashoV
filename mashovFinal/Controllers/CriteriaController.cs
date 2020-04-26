using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;

namespace mashovFinal.Controllers
{
    public class CriteriaController : ApiController
    {
        [HttpPut]
        [Route("api/Criteria")]
        public CritInDoc Put([FromBody]FeedBack_Doc d)
        {

            return d.getCrits();
        }
        [HttpPost]
        [Route("api/Criteria")]
        public int Post([FromBody]CritInDoc c)
        {

            return c.insert();
        }
        [HttpGet]
        [Route("api/Criteria")]
        public List<Criterion> Get()
        {
            Criterion c = new Criterion();
            return c.getAllCrit();
        }

    }
}
