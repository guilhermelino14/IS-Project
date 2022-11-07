using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
            string sqlString = "INSERT INTO applications values(@name, @creation_dt)";

            SqlCommand sqlCommand = new SqlCommand(sqlString);
            sqlCommand.Parameters.AddWithValue("@name", app.name);
            sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));

            postSomething(sqlCommand);

            return Ok();
        }

        // Read Applications
        [Route("")]
        public IHttpActionResult GetApplications()
        {
            List<Application> availableApps = new List<Application>();

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM applications ORDER BY id", conn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Application app = new Application
                    {
                        id = (int)reader["id"],
                        name = (string)reader["name"],
                        creation_dt = (string)reader["creation_dt"],
                    };
                    availableApps.Add(app);
                }

                reader.Close();
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

            return Ok(availableApps);
        }

        // Read Application by id
        [Route("{id:int}")]
        public IHttpActionResult GetApplicationById(int id)
        {
            SqlConnection conn = null;
            Application app = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                string testString = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";

                SqlCommand command = new SqlCommand(testString, conn);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    app = new Application
                    {
                        id = (int)reader["id"],
                        name = (string)reader["name"],
                        creation_dt = (string)reader["creation_dt"],
                    };
                }

                reader.Close();
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

            return Ok(app);
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
            deleteSomething("applications", id);

            return Ok();
        }

        // !!!!!!!!!!!!!!!
        // !!! Modules !!!
        // !!!!!!!!!!!!!!!

        // Create
        [Route("{module}")]
        public IHttpActionResult PostModule([FromBody] Module module)
        {
            string sqlString = "INSERT INTO modules values(@name, @creation_dt, @parent)";

            SqlCommand sqlCommand = new SqlCommand(sqlString);
            sqlCommand.Parameters.AddWithValue("@name", module.name);
            sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
            sqlCommand.Parameters.AddWithValue("@parent", module.parent);

            postSomething(sqlCommand);

            return Ok();
        }

        // Read Modules
        [Route("{module}")]
        public IHttpActionResult GetModules(string module)
        {
            List<Module> availableMods = new List<Module>();

            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM applications ORDER BY id", conn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Module mod = new Module
                    {
                        id = (int)reader["id"],
                        name = (string)reader["name"],
                        creation_dt = (string)reader["creation_dt"],
                        parent = (int)reader["parent"],
                    };
                    availableMods.Add(mod);
                }

                reader.Close();
                conn.Close();
            }
            catch (Exception e)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                return null;
            }

            return Ok(availableMods);
        }

        // Read Module by id
        [Route("{module}/{id:int}")]
        public IHttpActionResult GetModuleById(string module, int id)
        {
            SqlConnection conn = null;
            Module mod = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                string testString = "SELECT * FROM modules WHERE id = " + id + " ORDER BY id";

                SqlCommand command = new SqlCommand(testString, conn);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    mod = new Module
                    {
                        id = (int)reader["id"],
                        name = (string)reader["name"],
                        creation_dt = (string)reader["creation_dt"],
                        parent = (int)reader["parent"],
                    };
                }

                reader.Close();
                conn.Close();
            }
            catch (Exception e)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                return null;
            }

            return Ok(mod);
        }

        // Update
        [Route("{module}/{id:int}")]
        public IHttpActionResult PutModule(int id, [FromBody] Module model)
        {
            return Ok();
        }

        // Delete
        [Route("{module}/{id:int}")]
        public IHttpActionResult DeleteModule(int id)
        {
            deleteSomething("modules", id);

            return Ok();
        }

        // !!! Data & Subscriptions !!!
        // Create
        [Route("{module}/{value}")]
        // value can be Subscription or Data
        public IHttpActionResult PostSubModule([FromBody] Subscription model)
        {
            return Ok();
        }

        // Read
        [Route("{module}/{value}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModule(string module, string value)
        {
            return Ok();
        }

        // Update
        [Route("{module}/{value}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult PutSubModule(int id, [FromBody] Subscription model)
        {
            return Ok();
        }
        // Delete

        [Route("{module}/{value}/{id:int}")]
        public IHttpActionResult DeleteSubModule(int id)
        {
            return Ok();
        }


        // !!!!!!!!!!!!!!!!!!!!!!!
        // !! General Functions !!
        // !!!!!!!!!!!!!!!!!!!!!!!

        public void postSomething (SqlCommand sqlCommand)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                sqlCommand.Connection = sqlConnection;
                SqlCommand command = sqlCommand;

                int nrows = command.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void deleteSomething (string location, int id)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string str = "DELETE FROM " + location + " WHERE id = " + id;
                SqlCommand command = new SqlCommand(str, sqlConnection);

                int nrows = command.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

    }
}
