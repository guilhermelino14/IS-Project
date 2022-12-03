using Newtonsoft.Json.Linq;
using ProjectXML;
using SOMIOD.Models;
using SOMIOD.Properties;
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
using System.Runtime;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Http;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using Application = SOMIOD.Models.Application;
using Module = SOMIOD.Models.Module;

namespace SOMIOD.Controllers
{
    [RoutePrefix("api/somiod")]
    public class MainController : ApiController
    {
        
        public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SOMIOD.Properties.Settings.ConnStr"].ConnectionString;
        public static string GFILE_PATH = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\response.xml";
        public static string PFILE_PATH = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\post.xml";
        public static string XSD_PATH = HttpContext.Current.Server.MapPath("~/App_Data/");

        // !!!!!!!!!!!!!!!!!!!!
        // !!! Applications !!!
        // !!!!!!!!!!!!!!!!!!!!

        // Create
        [Route("")]
        public IHttpActionResult PostApplication()
        //public IHttpActionResult PostApplication([FromBody] XmlDocument doc)
        {
            string docPath = GetPostXmlPath();

            HandlerXML handler = new HandlerXML(docPath, XSD_PATH + "applicationVerification.xsd");

            if (!handler.ValidateXML())
            {
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }

            XmlDocument rcvDoc = new XmlDocument();
            rcvDoc.Load(docPath);
            XmlElement element = rcvDoc.DocumentElement;

            if (element.GetAttribute("res_type") == "application")
            {
                int appId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);

                string sqlQuery = "SELECT * FROM applications WHERE id = " + appId + " ORDER BY id";
                if (DbMethods.VerifyOnDB(sqlQuery, "applications") != appId)
                {
                    string appName = rcvDoc.SelectSingleNode("//name").InnerText;

                    string sqlString = "INSERT INTO applications values(@id, @name, @creation_dt)";

                    SqlCommand sqlCommand = new SqlCommand(sqlString);
                    sqlCommand.Parameters.AddWithValue("@id", appId);
                    sqlCommand.Parameters.AddWithValue("@name", appName);
                    sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));

                    DbMethods.ExecuteSqlCommand(sqlCommand);

                    return Ok("Application " + appName + " created successfully");
                }
                return Content(HttpStatusCode.BadRequest, "An application with id " + appId + " already exists");
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Read Applications
        [Route("")]
        public IHttpActionResult GetApplications()
        {
            string docPath = GetPostXmlPath();

            /*HandlerXML handler = new HandlerXML(docPath, XSD_PATH + "applicationVerification.xsd");

            if (!handler.ValidateXML())
            {
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }*/

            XmlDocument rcvDoc = new XmlDocument();
            rcvDoc.Load(docPath);
            XmlElement element = rcvDoc.DocumentElement;

            if (element.GetAttribute("res_type") == "application")
            {
                string sqlQuery = "SELECT * FROM applications ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "applications");

                return Ok(doc.OuterXml);
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Read Application by id
        [Route("{id:int}")]
        public IHttpActionResult GetApplicationById(int id)
        {
            string docPath = GetPostXmlPath();

            /*HandlerXML handler = new HandlerXML(docPath, XSD_PATH + "applicationVerification.xsd");

            if (!handler.ValidateXML())
            {
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }*/

            XmlDocument rcvDoc = new XmlDocument();
            rcvDoc.Load(docPath);
            XmlElement element = rcvDoc.DocumentElement;

            if (element.GetAttribute("res_type") == "application")
            {
                string sqlQuery = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "applications");

                return Ok(doc.OuterXml);
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Update
        [Route("{id:int}")]
        //public IHttpActionResult PutApplication(int id, [FromBody] Application app)
        public IHttpActionResult PutApplication(int id)
        {
            string docPath = GetPostXmlPath();

            /*HandlerXML handler = new HandlerXML(docPath, XSD_PATH + "applicationVerification.xsd");

            if (!handler.ValidateXML())
            {
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }*/

            XmlDocument rcvDoc = new XmlDocument();
            rcvDoc.Load(docPath);
            XmlElement element = rcvDoc.DocumentElement;

            if (element.GetAttribute("res_type") == "application")
            {
                Application oldApp = new Application();

                string sqlQuery = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "applications");
                string appName = rcvDoc.SelectSingleNode("//name").InnerText;
                oldApp.name = doc.SelectSingleNode("//name").InnerText;

                if (appName != oldApp.name)
                {
                    string sqlString = "UPDATE applications SET name = \'" + appName + "\' WHERE id = " + id;

                    SqlCommand sqlCommand = new SqlCommand(sqlString);

                    DbMethods.ExecuteSqlCommand(sqlCommand);

                    return Ok("Application " + id + " updated successfully");
                }
                return Content(HttpStatusCode.BadRequest, "Application " + id + " not updated");
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Delete
        [Route("{id:int}")]
        public IHttpActionResult DeleteApplication(int id)
        {
            string docPath = GetPostXmlPath();

            /*HandlerXML handler = new HandlerXML(docPath, XSD_PATH + "applicationVerification.xsd");

            if (!handler.ValidateXML())
            {
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }*/

            XmlDocument rcvDoc = new XmlDocument();
            rcvDoc.Load(docPath);
            XmlElement element = rcvDoc.DocumentElement;

            if (element.GetAttribute("res_type") == "application")
            {
                DbMethods.DeleteSomething("applications", id);

                return Ok("Application " + id + " deleted successfully");
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // !!!!!!!!!!!!!!!
        // !!! Modules !!!
        // !!!!!!!!!!!!!!!

        // Create
        [Route("{application}")]
        //public IHttpActionResult PostModule(string application, [FromBody] Module module)
        public IHttpActionResult PostModule(string application)
        {
            string docPath = GetPostXmlPath();

            HandlerXML handler = new HandlerXML(docPath, XSD_PATH + "moduleVerification.xsd");

            if (!handler.ValidateXML())
            {
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }

            XmlDocument rcvDoc = new XmlDocument();
            rcvDoc.Load(docPath);
            XmlElement element = rcvDoc.DocumentElement;

            if (element.GetAttribute("res_type") == "module")
            {
                int parentId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);

                string sqlQueryAppId = "SELECT * FROM applications WHERE id = \'" + parentId + "\'";
                int idParentApp = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

                if (idParentApp == 0)
                {
                    return Content(HttpStatusCode.BadRequest, application + " does not exist");
                }

                int moduleId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);

                string sqlQuery = "SELECT * FROM modules WHERE id = " + moduleId + " ORDER BY id";
                if (DbMethods.VerifyOnDB(sqlQuery, "modules") != moduleId)
                {
                    string moduleName = rcvDoc.SelectSingleNode("//name").InnerText;

                    string sqlString = "INSERT INTO modules values(@id, @name, @creation_dt, @parent)";

                    SqlCommand sqlCommand = new SqlCommand(sqlString);
                    sqlCommand.Parameters.AddWithValue("@id", moduleId);
                    sqlCommand.Parameters.AddWithValue("@name", moduleName);
                    sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                    sqlCommand.Parameters.AddWithValue("@parent", parentId);

                    //System.Diagnostics.Debug.WriteLine(sqlString);
                    DbMethods.ExecuteSqlCommand(sqlCommand);

                    return Ok("Module " + moduleName + " created successfully");
                }
                return Content(HttpStatusCode.BadRequest, "An module with id " + moduleId + " already exists");
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Read Modules
        [Route("{application}")]
        public IHttpActionResult GetModules(string application)
        {
            string res_type = "module";

            if (res_type == "module")
            {
                string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
                int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

                if (idApplication == 0)
                {
                    return Content(HttpStatusCode.BadRequest, application + " does not exist");
                }

                string sqlQuery = "SELECT * FROM modules WHERE parent = " + idApplication + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "modules");

                HandlerXML handler = new HandlerXML(GFILE_PATH, XSD_PATH + "moduleVerification.xsd");

                if (!handler.ValidateXML())
                {
                    return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
                }

                return Ok(doc);
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Read Module by id
        [Route("{application}/{id:int}")]
        public IHttpActionResult GetModuleById(string application, int id)
        {
            string res_type = "module";

            if (res_type == "module")
            {
                string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
                int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

                if (idApplication == 0)
                {
                    return Content(HttpStatusCode.BadRequest, application + " does not exist");
                }

                string sqlQuery = "SELECT * FROM modules WHERE id = " + id + " AND parent = " + idApplication + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "modules");

                HandlerXML handler = new HandlerXML(GFILE_PATH, XSD_PATH + "moduleVerification.xsd");

                if (!handler.ValidateXML())
                {
                    return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
                }

                return Ok(doc);
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Update
        [Route("{application}/{id:int}")]
        public IHttpActionResult PutModule(string application, int id, [FromBody] Module module)
        {
            string res_type = "module";

            if (res_type == "module")
            {
                string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
                int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

                if (idApplication == 0)
                {
                    return Content(HttpStatusCode.BadRequest, application + " does not exist");
                }

                Module oldModule = new Module();

                string sqlQuery = "SELECT * FROM modules WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "applications");

                oldModule.name = doc.SelectSingleNode("//name").InnerText;

                if (module.name != oldModule.name)
                {
                    string sqlString = "UPDATE modules SET name = \'" + module.name + "\' WHERE id = " + id;

                    SqlCommand sqlCommand = new SqlCommand(sqlString);

                    DbMethods.ExecuteSqlCommand(sqlCommand);

                    return Ok("Module " + id + " updated successfully");
                }
                return Content(HttpStatusCode.BadRequest, "Module " + id + " not updated");
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Delete
        [Route("{application}/{id:int}")]
        public IHttpActionResult DeleteModule(string application, int id)
        {
            string res_type = "module";

            if (res_type == "module")
            {
                string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
                int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

                if (idApplication == 0)
                {
                    return Content(HttpStatusCode.BadRequest, application + " does not exist");
                }

                DbMethods.DeleteSomething("modules", id);

                return Ok("Module " + id + " deleted successfully");
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // !!! Data & Subscriptions !!!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // Create
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult PostSubModule(string application, string module, [FromBody] Subscription model)
        {
            //string res_type = VerifyResType();
            string res_type = "";

            string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

            if (idApplication == 0)
            {
                return Content(HttpStatusCode.BadRequest, application + " does not exist");
            }

            string sqlQueryModuleId = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + idApplication;
            int idModule = DbMethods.VerifyOnDB(sqlQueryModuleId, "modules");

            if (idModule == 0)
            {
                return Content(HttpStatusCode.BadRequest, module + " does not exist on " + application);
            }

            if (res_type == "data")
            {
                string sqlString = "INSERT INTO data values(@content, @creation_dt, @parent)";

                SqlCommand sqlCommand = new SqlCommand(sqlString);
                sqlCommand.Parameters.AddWithValue("@content", model.name);
                sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                sqlCommand.Parameters.AddWithValue("@parent", idModule);

                System.Diagnostics.Debug.WriteLine(sqlString);
                DbMethods.ExecuteSqlCommand(sqlCommand);

                return Ok("Module created successfully");

            }
            else if (res_type == "subscription")
            {
                string sqlString = "INSERT INTO subscriptions values(@name, @creation_dt, @parent, @event, @endpoint)";

                SqlCommand sqlCommand = new SqlCommand(sqlString);
                sqlCommand.Parameters.AddWithValue("@name", model.name);
                sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                sqlCommand.Parameters.AddWithValue("@parent", idModule);
                sqlCommand.Parameters.AddWithValue("@event", model.subscription_event);
                sqlCommand.Parameters.AddWithValue("@endpoint", model.endpoint);

                System.Diagnostics.Debug.WriteLine(sqlString);
                DbMethods.ExecuteSqlCommand(sqlCommand);

                return Ok("Subscription created successfully");
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Read
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModule(string application, string module)
        {
            //string res_type = VerifyResType();
            string res_type = "";

            string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

            if (idApplication == 0)
            {
                return Content(HttpStatusCode.BadRequest, application + " does not exist");
            }

            string sqlQueryModuleId = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + idApplication;
            int idModule = DbMethods.VerifyOnDB(sqlQueryModuleId, "modules");

            if (idModule == 0)
            {
                return Content(HttpStatusCode.BadRequest, module + " does not exist on " + application);
            }

            if (res_type == "data")
            {
                string sqlQuery = "SELECT * FROM data WHERE parent = " + idModule + " ORDER BY id";
                XmlDocument responseXml = XmlUtils.GetSomething(sqlQuery, "data");

                HandlerXML handler = new HandlerXML(GFILE_PATH, XSD_PATH + "dataVerification.xsd");

                if (!handler.ValidateXML())
                {
                    return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
                }
                return Ok(responseXml);
            }
            else if (res_type == "subscription")
            {
                string sqlQuery = "SELECT * FROM subscriptions WHERE parent = " + idModule + " ORDER BY id";
                XmlDocument responseXml = XmlUtils.GetSomething(sqlQuery, "subscriptions");

                HandlerXML handler = new HandlerXML(GFILE_PATH, XSD_PATH + "subscriptionVerification.xsd");

                if (!handler.ValidateXML())
                {
                    return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
                }
                return Ok(responseXml);
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Read
        [Route("{application}/{module}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult GetSubModuleById(string application, string module, int id)
        {
            //string res_type = VerifyResType();
            string res_type = "";

            string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

            if (idApplication == 0)
            {
                return Content(HttpStatusCode.BadRequest, application + " does not exist");
            }

            string sqlQueryModuleId = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + idApplication;
            int idModule = DbMethods.VerifyOnDB(sqlQueryModuleId, "modules");

            if (idModule == 0)
            {
                return Content(HttpStatusCode.BadRequest, module + " does not exist on " + application);
            }

            if (res_type == "data")
            {
                string sqlQuery = "SELECT * FROM data WHERE id = " + id + " AND parent = " + idModule + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "data");

                return Ok(doc);
            }
            else if (res_type == "subscription")
            {
                string sqlQuery = "SELECT * FROM subscriptions WHERE id = " + id + " AND parent = " + idModule + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "subscriptions");

                return Ok(doc);
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Update
        [Route("{application}/{module}/{id:int}")]
        // value can be Subscription or Data
        public IHttpActionResult PutSubModule(string application, string module, int id, [FromBody] Subscription model)
        {
            //string res_type = VerifyResType();
            string res_type = "";

            string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

            if (idApplication == 0)
            {
                return Content(HttpStatusCode.BadRequest, application + " does not exist");
            }

            string sqlQueryModuleId = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + idApplication;
            int idModule = DbMethods.VerifyOnDB(sqlQueryModuleId, "modules");

            if (idModule == 0)
            {
                return Content(HttpStatusCode.BadRequest, module + " does not exist on " + application);
            }

            if (res_type == "data")
            {
                Data oldData = new Data();

                string sqlQuery = "SELECT * FROM data WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "data");

                oldData.content = doc.SelectSingleNode("//content").InnerText;

                if (model.name != oldData.content)
                {
                    string sqlString = "UPDATE data SET content = \'" + model.name + "\' WHERE id = " + id;

                    SqlCommand sqlCommand = new SqlCommand(sqlString);

                    DbMethods.ExecuteSqlCommand(sqlCommand);

                    return Ok("Data " + id + " updated successfully");
                }
                return Ok("No need for update on data " + id);
            }
            else if (res_type == "subscription")
            {
                Subscription oldSubscription = new Subscription();

                string sqlQuery = "SELECT * FROM subscriptions WHERE id = " + id + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "subscriptions");

                oldSubscription.name = doc.SelectSingleNode("//name").InnerText;

                /*if ()
                {
                    return Ok("Subscription " + id + " updated successfully");
                }*/
                return Ok("No need for update on subscription " + id);
            }
            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }

        // Delete
        [Route("{application}/{module}/{id:int}")]
        public IHttpActionResult DeleteSubModule(string application, string module, int id)
        {
            //string res_type = VerifyResType();
            string res_type = "";

            string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int idApplication = DbMethods.VerifyOnDB(sqlQueryAppId, "applications");

            if (idApplication == 0)
            {
                return Content(HttpStatusCode.BadRequest, application + " does not exist");
            }

            string sqlQueryModuleId = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + idApplication;
            int idModule = DbMethods.VerifyOnDB(sqlQueryModuleId, "modules");

            if (idModule == 0)
            {
                return Content(HttpStatusCode.BadRequest, module + " does not exist on " + application);
            }

            if (res_type == "data")
            {
                DbMethods.DeleteSomething("data", id);

                return Ok("Data " + id + " deleted successfully");
            }
            else if (res_type == "subscription")
            {
                DbMethods.DeleteSomething("subscriptions", id);

                return Ok("Subscription " + id + " deleted successfully");
            }

            return Content(HttpStatusCode.BadRequest, "Invalid res_type");
        }


        // !!!!!!!!!!!!!!!!!!!!!!!
        // !! General Functions !!
        // !!!!!!!!!!!!!!!!!!!!!!!


        public static string GetPostXmlPath()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();

            StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\post.xml");
            writer.Write(bodyText);
            writer.Close();

            return PFILE_PATH;
        }

    }
}
