using BVNVerification.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BVNVerification.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        [Route("api/test")]
        [HttpPost]
        public string Test([FromBody] BVNClass bvn)
        {
            return "Test";
        }
        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
