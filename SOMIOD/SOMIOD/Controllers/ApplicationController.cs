using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class ApplicationController : ApiController
    {
        // Create
        // Read
        // Update
        // Delete

        [Route("")]
        public IHttpActionResult GetApplication()
        {
            return Ok();
        }
    }
}
