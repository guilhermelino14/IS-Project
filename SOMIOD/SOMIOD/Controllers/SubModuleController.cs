using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SubModuleController : ApiController
    {
        // Create
        // Read
        // Update
        // Delete
        
        [Route("{module}/{value}")]
        // value can be Subscription or Data
        public IHttpActionResult GetApplication(string module, string value)
        {
            System.Diagnostics.Debug.WriteLine(module);
            System.Diagnostics.Debug.WriteLine(value);
            return Ok();
        }
    }
}
