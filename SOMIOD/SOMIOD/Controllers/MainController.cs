using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class MainController : ApiController
    {
        // !!! Applications !!!
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

        // !!! Modules !!!
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

        // !!! Data & Subscriptions !!!
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
