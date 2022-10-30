using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class ModuleController : ApiController
    {
        // Create
        // Read
        // Update
        // Delete

        [Route("{module}")]
        public IHttpActionResult GetApplication(string module)
        {
            System.Diagnostics.Debug.WriteLine(module);
            return Ok();
        }
    }
}
