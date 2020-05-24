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
        [HttpPost] 
        [Route("api/Criteria/LastnewDoc")]
        public int PostLastnewDoc([FromBody]CritInDoc c)
        {

            return c.updateDocLastDoc();
        }
        [HttpGet]
        [Route("api/Criteria")]
        public List<Criterion> Get()
        {
            Criterion c = new Criterion();
            return c.getAllCrit();
        }
       
        [HttpGet]
        [Route("api/Criteria/judge/{numMeet}")]
        public CritInDoc Get(int numMeet)
        {
            CritInDoc c = new CritInDoc();
            return c.getTest(numMeet);
        }
        [HttpPut]
        [Route("api/Criteria/Group/{numMeet}")]
        public CritInDoc Pur(int numMeet,[FromBody] Users judge)
        {
            CritInDoc c = new CritInDoc();
            return c.getTestupdate(numMeet,judge);
        }
        [HttpPut]
        [Route("api/Criteria/Last")]
        public List<CritInDoc> Put( [FromBody] CoursesAndDepartment j)
        {
            CritInDoc c = new CritInDoc();
            return c.getLast(j);
        }
        [HttpPost]
        [Route("api/Criteria/delete")]
        public int PostDelete([FromBody] CritInDoc cd)
        {
            
            return cd.deleteCritFromDoc();
        }
    }
}
