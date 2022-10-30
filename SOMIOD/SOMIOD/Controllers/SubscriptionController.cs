using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SubscriptionController : ApiController
    {
        // Create
        // Read
        // Update
        // Delete

        [Route("{module}/{subscription}")]
        public IHttpActionResult GetApplication(string module, string subscription)
        {
            System.Diagnostics.Debug.WriteLine(module);
            System.Diagnostics.Debug.WriteLine(subscription);
            return Ok();
        }
    }
}
