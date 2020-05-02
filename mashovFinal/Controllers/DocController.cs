using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;


namespace mashovFinal.Controllers
{
    public class DocController : ApiController
    {
        [HttpGet]
        [Route("api/Doc")]
        public List<Department> Get()
        {
            Department d = new Department();
            return d.getList();
        }
        [HttpGet]
        [Route("api/Doc/Year")]
        public List<string> GetYear()
        {
            Department d = new Department();
            return d.getYears();
        }
        [HttpGet]
        [Route("api/Doc/Cours")]
        public List<Courses> GetCours()
        {
            Courses c = new Courses();
            return c.getCourses();
        }
        [HttpGet]
        [Route("api/Doc/Scala")]
        public List<Scala> GetScala()
        {
            Scala s = new Scala();
            return s.getScala();
        }
        [HttpPost]
        [Route("api/Doc/UpdateCrit")]
        public int PostUpdateCrit([FromBody] CritInDoc c)
        {

            return c.updateAndinsertCrit();
        }
        [HttpPost]
        [Route("api/Doc")]
        public int Post([FromBody] FullFeedback f)
        {

            return f.insert();
        }
    }
}
