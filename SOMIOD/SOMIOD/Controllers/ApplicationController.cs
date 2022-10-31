using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SOMIOD.Models;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class ApplicationController : ApiController
    {
        // Create
        [Route("")]
        public IHttpActionResult PostApplication([FromBody] Application app)
        {
            return Ok();
        }

        // Read
        [Route("")]
        public IHttpActionResult GetApplication()
        {
            return Ok();
        }

        // Update
        [Route("{id:int}")]
        public IHttpActionResult PutApplication(int id, [FromBody] Application app)
        {
            System.Diagnostics.Debug.WriteLine("asd");
            return Ok();
        }

        // Delete
        [Route("{id:int}")]
        public IHttpActionResult DeleteApplication(int id)
        {
            return Ok();
        }
    }
}
