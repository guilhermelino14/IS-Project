using Newtonsoft.Json.Linq;
using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Application = SOMIOD.Models.Application;
using Module = SOMIOD.Models.Module;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class MainController : ApiController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SOMIOD.Properties.Settings.ConnStr"].ConnectionString;
        
        //string FILE_PATH = "C:\\Users\\Guilherme Lino\\Desktop\\IPL\\IS\\IS-Project\\response.xml";
        string FILE_PATH = "C:\\Users\\marco\\Desktop\\GitHub\\IS-Project\\response.xml";

        // !!! Applications !!!
        // Create
        [Route("")]
        public IHttpActionResult PostApplication([FromBody] Application app)
        {
            System.Diagnostics.Debug.WriteLine(app);
            string sqlString = "INSERT INTO applications values(@name, @creation_dt)";

            SqlCommand sqlCommand = new SqlCommand(sqlString);
            sqlCommand.Parameters.AddWithValue("@name", app.name);
            sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));

            ExecuteSqlCommand(sqlCommand);

            return Ok();
        }

        // Read Applications
        [Route("")]
        public IHttpActionResult GetApplications()
        {
            string sqlQuery = "SELECT * FROM applications ORDER BY id";
            XmlDocument doc = GetSomething(sqlQuery, "Applications");

            return Ok(doc);
        }

        // Read Application by id
        [Route("{id:int}")]
        public IHttpActionResult GetApplicationById(int id)
        {
            string sqlQuery = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
            XmlDocument doc = GetSomething(sqlQuery, "Applications");

            return Ok(doc);
        }

        // Update
        [Route("{id:int}")]
        public IHttpActionResult PutApplication(int id, [FromBody] Application app)
        {
            Application oldApp = new Application();

            string sqlQuery = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
            XmlDocument doc = GetSomething(sqlQuery, "Applications");

            oldApp.name = doc.SelectSingleNode("//name").InnerText;

            if (app.name != oldApp.name)
            {
                string sqlString = "UPDATE applications SET name = \'" + app.name + "\' WHERE id = " + id;

                SqlCommand sqlCommand = new SqlCommand(sqlString);

                ExecuteSqlCommand(sqlCommand);
            }

            return Ok();
        }

        // Delete
        [Route("{id:int}")]
        public IHttpActionResult DeleteApplication(int id)
        {
            DeleteSomething("applications", id);

            return Ok();
        }

        // !!!!!!!!!!!!!!!
        // !!! Modules !!!
        // !!!!!!!!!!!!!!!

        // Create
        [Route("{application}")]
        public IHttpActionResult PostModule([FromBody] Module module)
        {
            string sqlString = "INSERT INTO modules values(@name, @creation_dt, @parent)";

            SqlCommand sqlCommand = new SqlCommand(sqlString);
            sqlCommand.Parameters.AddWithValue("@name", module.name);
            sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
            sqlCommand.Parameters.AddWithValue("@parent", module.parent);

            System.Diagnostics.Debug.WriteLine(sqlString);
            ExecuteSqlCommand(sqlCommand);

            return Ok();
        }

        // Read Modules
        [Route("{application}")]
        public IHttpActionResult GetModules(string application)
        {
            string sqlQueryGetApplicationID = "SELECT * FROM applications WHERE name = \'" + application +"\'";
            XmlDocument docApplication = GetSomething(sqlQueryGetApplicationID, "Applications");
            int idApplication = Convert.ToInt32(docApplication.SelectSingleNode("//id").InnerText);

            string sqlQuery = "SELECT * FROM modules WHERE parent = " + idApplication + " ORDER BY id";
            XmlDocument doc = GetSomething(sqlQuery, "Modules");

            return Ok(doc);
        }

        // Read Module by id
        [Route("{module}/{id:int}")]
        public IHttpActionResult GetModuleById(string module, int id)
        {
            string sqlQuery = "SELECT * FROM modules WHERE id = " + id + " ORDER BY id";
            XmlDocument doc = GetSomething(sqlQuery, "Modules");

            return Ok(doc);
        }

        // Update
        [Route("{module}/{id:int}")]
        public IHttpActionResult PutModule(int id, [FromBody] Module module)
        {
            Module oldModule = new Module();

            string sqlQuery = "SELECT * FROM modules WHERE id = " + id + " ORDER BY id";
            XmlDocument doc = GetSomething(sqlQuery, "Applications");

            oldModule.name = doc.SelectSingleNode("//name").InnerText;

            if (module.name != oldModule.name)
            {
                string sqlString = "UPDATE modules SET name = \'" + module.name + "\' WHERE id = " + id;

                SqlCommand sqlCommand = new SqlCommand(sqlString);

                ExecuteSqlCommand(sqlCommand);
            }

            return Ok();
        }

        // Delete
        [Route("{application}/{id:int}")]
        public IHttpActionResult DeleteModule(int id)
        {
            DeleteSomething("modules", id);

            return Ok();
        }

        // !!! Data & Subscriptions !!!
        // Create
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult PostSubModule([FromBody] Subscription model, string module)
        {
            string res_type = VerifyResType();

            string sqlQueryGetModuleID = "SELECT * FROM modules WHERE name = \'" + module + "\'";
            XmlDocument docAModule = GetSomething(sqlQueryGetModuleID, "Modules");
            int idModule = Convert.ToInt32(docAModule.SelectSingleNode("//id").InnerText);

            if (idModule == model.parent)
            {
                if (res_type == "data")
                {
                    string sqlString = "INSERT INTO data values(@content, @creation_dt, @parent)";

                    SqlCommand sqlCommand = new SqlCommand(sqlString);
                    sqlCommand.Parameters.AddWithValue("@content", model.name);
                    sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                    sqlCommand.Parameters.AddWithValue("@parent", model.parent);

                    System.Diagnostics.Debug.WriteLine(sqlString);
                    ExecuteSqlCommand(sqlCommand);

                }
                else if (res_type == "subscription")
                {
                    string sqlString = "INSERT INTO subscriptions values(@name, @creation_dt, @parent, @event, @endpoint)";

                    SqlCommand sqlCommand = new SqlCommand(sqlString);
                    sqlCommand.Parameters.AddWithValue("@name", model.name);
                    sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                    sqlCommand.Parameters.AddWithValue("@parent", model.parent);
                    sqlCommand.Parameters.AddWithValue("@event", model.subscription_event);
                    sqlCommand.Parameters.AddWithValue("@endpoint", model.endpoint);

                    System.Diagnostics.Debug.WriteLine(sqlString);
                    ExecuteSqlCommand(sqlCommand);
                }
            }

            return Ok(res_type + " created successfully!");
            //return Ok(docAModule);
        }

        // Read
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModule(string application, string module)
        {
            string res_type = VerifyResType();

            if (res_type == "data")
            {
                string sqlQuery = "SELECT * FROM data ORDER BY id";
                XmlDocument doc = GetSomething(sqlQuery, "Data");

            }
            else if (res_type == "subscription")
            {
                string sqlQuery = "SELECT * FROM subscriptions ORDER BY id";
                XmlDocument doc = GetSomething(sqlQuery, "Subscriptions");
            }

            return Ok();
        }

        // Read
        [Route("{application}/{module}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModuleById(string application, string module, int id)
        {
            string res_type = VerifyResType();

            if (res_type == "data")
            {
                string sqlQuery = "SELECT * FROM data WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = GetSomething(sqlQuery, "Data");

            }
            else if (res_type == "subscription")
            {
                string sqlQuery = "SELECT * FROM subscriptions WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = GetSomething(sqlQuery, "Subscriptions");
            }

            return Ok();
        }

        // Update
        [Route("{application}/{module}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult PutSubModule(int id, [FromBody] Subscription model)
        {
            string res_type = VerifyResType();

            if (res_type == "data")
            {
                Data oldData = new Data();

                string sqlQuery = "SELECT * FROM data WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = GetSomething(sqlQuery, "Data");

                oldData.content = doc.SelectSingleNode("//content").InnerText;

                if (model.name != oldData.content)
                {
                    string sqlString = "UPDATE data SET content = \'" + model.name + "\' WHERE id = " + id;

                    SqlCommand sqlCommand = new SqlCommand(sqlString);

                    ExecuteSqlCommand(sqlCommand);
                }

            }
            else if (res_type == "subscription")
            {
                Subscription oldSubscription = new Subscription();

                string sqlQuery = "SELECT * FROM subscriptions WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = GetSomething(sqlQuery, "Subscriptions");

                oldSubscription.name = doc.SelectSingleNode("//name").InnerText;

            }

            return Ok();
        }

        // Delete
        [Route("{application}/{module}/{id:int}")]
        public IHttpActionResult DeleteSubModule(int id)
        {
            string res_type = VerifyResType();

            if (res_type == "data")
            {
                DeleteSomething("data", id);
            }
            else if (res_type == "subscription")
            {
                DeleteSomething("subscriptions", id);
            }

            return Ok();
        }


        // !!!!!!!!!!!!!!!!!!!!!!!
        // !! General Functions !!
        // !!!!!!!!!!!!!!!!!!!!!!!

        public void ExecuteSqlCommand (SqlCommand sqlCommand)
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

        public XmlDocument GetSomething (string sqlString, string type)
        {
            SqlConnection conn = null;

            XmlDocument doc = CreateXML();
            XmlElement root = doc.CreateElement(type);
            doc.AppendChild(root);

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand command = new SqlCommand(sqlString, conn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    switch (type) {
                        case ("Applications"):
                            Application app = FillApplication(reader);
                            root.AppendChild(CreateApplication(doc, app.id, app.name, app.creation_dt, "Application"));
                            break;
                        case ("Modules"):
                            Module mod = FillModule(reader);
                            root.AppendChild(CreateModule(doc, mod.id, mod.name, mod.creation_dt, mod.parent, "Module"));
                            break;
                        case ("Data"):
                            Data data = FillData(reader);
                            root.AppendChild(CreateData(doc, data.id, data.content, data.creation_dt, data.parent, "Data"));
                            break;
                        case ("Subscriptions"):
                            Subscription sub = FillSubscription(reader);
                            root.AppendChild(CreateSubscription(doc, sub.id, sub.name, sub.creation_dt, sub.parent, sub.subscription_event, sub.endpoint, "Subscription"));
                            break;
                    }
                    doc.Save(FILE_PATH);
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
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return doc;
        }

        public void DeleteSomething (string location, int id)
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

        public XmlDocument CreateXML()
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", null, null);
            xmlDocument.AppendChild(xmlDeclaration);

            return xmlDocument;
        }

        public XmlElement CreateApplication(XmlDocument doc, int id, string name, string creation_dt, string type)
        {
            XmlElement application = doc.CreateElement(type);

            XmlElement idE = doc.CreateElement("id");
            idE.InnerText = id.ToString();
            XmlElement nameE = doc.CreateElement("name");
            nameE.InnerText = name;
            XmlElement creation_dtE = doc.CreateElement("creation_dt");
            creation_dtE.InnerText = creation_dt;

            application.AppendChild(idE);
            application.AppendChild(nameE);
            application.AppendChild(creation_dtE);

            return application;
        }

        public XmlElement CreateModule(XmlDocument doc, int id, string name, string creation_dt, int parent, string type)
        {
            XmlElement module = CreateApplication(doc, id, name, creation_dt, type);

            XmlElement parentE = doc.CreateElement("parent");
            parentE.InnerText = parent.ToString();

            module.AppendChild(parentE);

            return module;
        }

        public XmlElement CreateData(XmlDocument doc, int id, string content, string creation_dt, int parent, string type)
        {
            XmlElement data = doc.CreateElement(type);

            XmlElement idE = doc.CreateElement("id");
            idE.InnerText = id.ToString();
            XmlElement contentE = doc.CreateElement("content");
            contentE.InnerText = content;
            XmlElement creation_dtE = doc.CreateElement("creation_dt");
            creation_dtE.InnerText = creation_dt;
            XmlElement parentE = doc.CreateElement("parent");
            parentE.InnerText = parent.ToString();

            data.AppendChild(idE);
            data.AppendChild(contentE);
            data.AppendChild(creation_dtE);
            data.AppendChild(parentE);

            return data;
        }

        public XmlElement CreateSubscription(XmlDocument doc, int id, string name, string creation_dt, int parent, string eve, string endpoint, string type)
        {
            XmlElement module = CreateModule(doc, id, name, creation_dt, parent, type);

            XmlElement eventE = doc.CreateElement("event");
            eventE.InnerText = eve;
            XmlElement endpointE = doc.CreateElement("endpoint");
            endpointE.InnerText = endpoint;

            module.AppendChild(eventE);
            module.AppendChild(endpointE);

            return module;
        }

        public Application FillApplication(SqlDataReader reader)
        {
            Application application = new Application
            {
                id = (int)reader["id"],
                name = (string)reader["name"],
                creation_dt = (string)reader["creation_dt"],
            };
            return application;
        }

        public Module FillModule(SqlDataReader reader)
        {
            Module module = new Module
            {
                id = (int)reader["id"],
                name = (string)reader["name"],
                creation_dt = (string)reader["creation_dt"],
                parent = (int)reader["parent"],
            };
            return module;
        }

        public Data FillData(SqlDataReader reader)
        {
            Data data = new Data
            {
                id = (int)reader["id"],
                content = (string)reader["content"],
                creation_dt = (string)reader["creation_dt"],
                parent = (int)reader["parent"],
            };
            return data;
        }

        public Subscription FillSubscription(SqlDataReader reader)
        {
            Subscription subscription = new Subscription
            {
                id = (int)reader["id"],
                name = (string)reader["content"],
                creation_dt = (string)reader["creation_dt"],
                parent = (int)reader["parent"],
                subscription_event = (string)reader["event"],
                endpoint = (string)reader["endpoint"],
            };
            return subscription;
        }

        public string VerifyResType ()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            var bodyJson = JObject.Parse(bodyText);
            var res_type = bodyJson["res_type"];
            System.Diagnostics.Debug.WriteLine(res_type);

            return res_type.ToString().ToLower();
        }
    }
}
