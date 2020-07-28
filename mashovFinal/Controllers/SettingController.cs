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
        //---------------get start details---------//
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
        //api/Setting/getjudge/
        [HttpGet]
        [Route("api/Setting/getjudge/{num}")]
        public List<Users> getjudge(int num)
        {
            Users u = new Users();
            return u.getAllJ(num);
        }



        //==================insert -==================//

        //------new user------------//
        [HttpPost]
        [Route("api/Setting/newUser")]
        public int postnewUser([FromBody] Users u)
        { 
            return u.insertNewUser();
        }

        //-------new group------//


        //===================adding =====================//

        //-------------------add judge to group----------//
        [HttpPost]
        [Route("api/Setting/judgeInGroup/")]
        public int postjudgeInGroup([FromBody] Group_Meeting j)
        {
            return j.insertjudgeInGroup();
        }
        //--------------------add stud to group----------//
        [HttpPost]
        [Route("api/Setting/studINGroup")]
        public int poststudINGroup([FromBody] Students s)
        {
            
            return s.studINGroup();
        }

        //===========delete===========================//

        //-----------------delete Stud from group------------//

        [HttpPut]
        [Route("api/Setting/deleteStud")]
        public int putdeleteStud([FromBody] Students u)
        {
            return u.deleteStud();
        }

        //----------------delete judge from group----------//

        [HttpPut]
        [Route("api/Setting/deletejudge")]
        public int putdeletejudge([FromBody] Group_Meeting u)
        {
            return u.deletejudge();
        }


        //================insert new Cours || new Department============// 
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


        //[HttpPut]
        //[Route("api/Setting/Student")]
        //public int put([FromBody] Students s)
        //{
        //    return s.updateGroupsOfStudent();
        //}



        //[HttpGet]
        //[Route("api/Setting/Type")]
        //public List<Types> getType()
        //{
        //    Types s = new Types();
        //    return s.getAlltypes();
        //}
    }
}
