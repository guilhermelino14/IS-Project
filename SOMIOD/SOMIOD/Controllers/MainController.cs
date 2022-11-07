using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class MainController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SOMIOD.Properties.Settings.ConnStr"].ConnectionString;

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
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                conn.Close();
            }
            catch (Exception e)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return null;
            }

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
        [Route("{application}")]
        public IHttpActionResult PostModule([FromBody] Application application)
        {
            return Ok();
        }

        // Read
        [Route("{application}")]
        public IHttpActionResult GetModule(string application)
        {
            return Ok();
        }

        // Update
        [Route("{application}/{id:int}")]
        public IHttpActionResult PutModule(int id, [FromBody] Application application)
        {
            return Ok();
        }

        // Delete
        [Route("{application}/{id:int}")]
        public IHttpActionResult DeleteModule(int id)
        {
            return Ok();
        }

        // !!! Data & Subscriptions !!!
        // Create
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult PostSubModule([FromBody] Subscription model)
        {
            return Ok();
        }

        // Read
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModule(string module, string value)
        {
            return Ok();
        }

        // Update
        [Route("{application}/{module}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult PutSubModule(int id, [FromBody] Subscription model)
        {
            return Ok();
        }
        // Delete

        [Route("{application}/{module}/{id:int}")]
        public IHttpActionResult DeleteSubModule(int id)
        {
            return Ok();
        }
    }
}
