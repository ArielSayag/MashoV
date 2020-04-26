using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using mashovFinal.Models;


namespace mashovFinal.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPut]
        [Route("api/Login/User")]
        public List<Users> Put([FromBody]Users u)
        {
            
            return u.ifexist();
        }
        [HttpPut]
        [Route("api/Login/Student")]
        public Students PutStudent([FromBody]Students s)
        {
            
            return s.checkingExist();
        }
        [HttpPut]
        [Route("api/Login/Pass")]
        public int PutPass([FromBody]Users u)
        {

            return u.update();
        }
        [HttpPut]
        [Route("api/Login/PassStudent")]
        public int PutPassStudent([FromBody]Students s)
        {

            return s.update();
        }
      

    }
}
