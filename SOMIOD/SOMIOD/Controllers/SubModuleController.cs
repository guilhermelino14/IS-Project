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
    public class SubModuleController : ApiController
    {
        // Create
        [Route("{module:alpha}/{value:alpha}")]
        // value can be Subscription or Data
        public IHttpActionResult PostSubModule([FromBody] Subscription model)
        {
            System.Diagnostics.Debug.WriteLine("asd");
            return Ok();
        }

        // Read
        [Route("{module:alpha}/{value:alpha}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModule(string module, string value)
        {
            System.Diagnostics.Debug.WriteLine(module);
            System.Diagnostics.Debug.WriteLine(value);
            return Ok();
        }

        // Update
        [Route("{module:alpha}/{value:alpha}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult PutSubModule(int id, [FromBody] Subscription model)
        {
            System.Diagnostics.Debug.WriteLine("asd");
            return Ok();
        }
        // Delete

        [Route("{module:alpha}/{value:alpha}/{id:int}")]
        public IHttpActionResult DeleteSubModule(int id)
        {
            System.Diagnostics.Debug.WriteLine("asd");
            return Ok();
        }


    }
}
