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
    public class ModuleController : ApiController
    {
        // Create
        [Route("{module:alpha}")]
        public IHttpActionResult PostModule([FromBody] Model model)
        {
            System.Diagnostics.Debug.WriteLine(model);
            return Ok();
        }
        
        // Read
        [Route("{module:alpha}")]
        public IHttpActionResult GetModule(string module)
        {
            System.Diagnostics.Debug.WriteLine(module);
            return Ok();
        }

        // Update
        [Route("{module:alpha}/{id:int}")]
        public IHttpActionResult PutModule(int id, [FromBody] Model model)
        {
            System.Diagnostics.Debug.WriteLine("asd");
            return Ok();
        }

        // Delete
        [Route("{module:alpha}/{id:int}")]
        public IHttpActionResult DeleteModule(int id)
        {
            System.Diagnostics.Debug.WriteLine("asd");
            return Ok();
        }

    }
}
