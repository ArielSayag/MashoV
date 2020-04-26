using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;

namespace mashovFinal.Controllers
{
    public class SettingController : ApiController
    {
        
        [HttpGet]
        [Route("api/Setting/Group/{name}")]
        public List<Group_Meeting> getGroup(int name)
        {
            Groups s = new Groups();
            return s.getAllgroups(name);
        }
        [HttpGet]
        [Route("api/Setting/User")]
        public List<Users> getUser()
        {
            Users s = new Users();
            return s.getAllusers();
        }
        [HttpGet]
        [Route("api/Setting/Student/{name}")]
        public List<Students> getStudent(int name)
        {
            Students s = new Students();
            return s.getAllstudents(name);
        }
        [HttpGet]
        [Route("api/Setting/Type")]
        public List<Types> getType()
        {
            Types s = new Types();
            return s.getAlltypes();
        }
        [HttpPost]
        [Route("api/Setting/newUser")]
        public int postnewUser([FromBody] Users u)
        { 
            return u.insertNewUser();
        }
        
        [HttpPost]
        [Route("api/Setting/judgeInGroup/")]
        public int postjudgeInGroup([FromBody] Group_Meeting j)
        {
            return j.insertjudgeInGroup();
        }
       
        [HttpPut]
        [Route("api/Setting/Student")]
        public int put([FromBody] Students s)
        {
            return s.updateGroupsOfStudent();
        }
        [HttpPost]
        [Route("api/Setting/Course")]
        public int postCourse([FromBody] string course_name)
        {
            Courses c = new Courses();
            return c.insert(course_name);
        }
        [HttpPost]
        [Route("api/Setting/Dep")]
        public int postDep([FromBody] string dep_name)
        {
            Department d = new Department();
            return d.insert(dep_name);
        }
    }
}
