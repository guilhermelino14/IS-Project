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
        public static string RESPONSE_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\response.xml";
        public static string RECEIVED_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\received.xml";
        public static string XSD_PATH = HttpContext.Current.Server.MapPath("~/App_Data/");
        public static string RES_TYPE_ERROR = "Invalid res_type, did you mean: res_type = ";

        // !!!!!!!!!!!!!!!!!!!!
        // !!! Applications !!!
        // !!!!!!!!!!!!!!!!!!!!

        // Create
        [Route("")]
        public IHttpActionResult PostApplication()
        {
            GetXmlFromStream();

            HandlerXML handler = new HandlerXML(RECEIVED_FILE_PATH, XSD_PATH + "applicationVerification.xsd");

            if (handler.ValidateXML()) // Se o XML estiver certo com o XSD
            {
                XmlDocument rcvDoc = new XmlDocument();
                rcvDoc.Load(RECEIVED_FILE_PATH);
                XmlElement element = rcvDoc.DocumentElement;

                if (element.GetAttribute("res_type") == "application") // Se o res_type for o requerido
                {
                    int appId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);
                    string sqlQuery = "SELECT * FROM applications WHERE id = " + appId + " ORDER BY id";

                    if (DbMethods.GetId(sqlQuery, "applications") != appId) // Se não existir outra aplicação com o mesmo id
                    {
                        string appName = rcvDoc.SelectSingleNode("//name").InnerText;

                        string sqlString = "INSERT INTO applications values(@id, @name, @creation_dt)";

                        SqlCommand sqlCommand = new SqlCommand(sqlString);
                        sqlCommand.Parameters.AddWithValue("@id", appId);
                        sqlCommand.Parameters.AddWithValue("@name", appName);
                        sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));

                        DbMethods.ExecuteSqlCommand(sqlCommand);

                        return Ok("Application " + appName + " created successfully with id " + appId);
                    }
                    return Content(HttpStatusCode.BadRequest, "An application with id " + appId + " already exists");
                }
                return Content(HttpStatusCode.BadRequest, RES_TYPE_ERROR + "application ?");
            }
            return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
        }

        // Read Applications
        [Route("")]
        public IHttpActionResult GetApplications()
        {
            string sqlQuery = "SELECT * FROM applications ORDER BY id";
            XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "applications");

            if (doc.SelectSingleNode("//application") == null)
            {
                return Content(HttpStatusCode.BadRequest, "No applications available");
            }

            return Ok(doc.OuterXml);
        }

        // Read Application by id
        [Route("{id:int}")]
        public IHttpActionResult GetApplicationById(int id)
        {
            string sqlQueryAppId = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";

            if (DbMethods.GetId(sqlQueryAppId, "applications") == id)
            {
                XmlDocument doc = XmlUtils.GetSomething(sqlQueryAppId, "applications");

                return Ok(doc.OuterXml);
            }
            return Content(HttpStatusCode.BadRequest, "Application " + id + " doesn't exist");
        }

        // Update
        [Route("{id:int}")]
        public IHttpActionResult PutApplication(int id)
        {
            string sqlQueryAppId = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
            if (DbMethods.GetId(sqlQueryAppId, "applications") == id)
            {
                GetXmlFromStream();

                HandlerXML handler = new HandlerXML(RECEIVED_FILE_PATH, XSD_PATH + "applicationVerification.xsd");

                if (handler.ValidateXML())
                {
                    XmlDocument rcvDoc = new XmlDocument();
                    rcvDoc.Load(RECEIVED_FILE_PATH);
                    XmlElement element = rcvDoc.DocumentElement;

                    if (element.GetAttribute("res_type") == "application")
                    {
                        string appName = rcvDoc.SelectSingleNode("//name").InnerText;

                        Application oldApp = new Application();

                        //string sqlQuery = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
                        XmlDocument doc = XmlUtils.GetSomething(sqlQueryAppId, "applications");
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
                    return Content(HttpStatusCode.BadRequest, RES_TYPE_ERROR + "application ?");
                }
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }
            return Content(HttpStatusCode.BadRequest, "Application " + id + " doesn't exist");
        }

        // Delete
        [Route("{id:int}")]
        public IHttpActionResult DeleteApplication(int id)
        {
            string sqlQueryAppId = "SELECT * FROM applications WHERE id = " + id + " ORDER BY id";
            if (DbMethods.GetId(sqlQueryAppId, "applications") == id)
            {
                string sqlQuery = "SELECT * FROM modules WHERE parent = " + id + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "modules");

                foreach (XmlNode node in doc.SelectNodes("//id")){
                    if (node.InnerText != null)
                    {
                        DeleteModule(DbMethods.GetName(sqlQueryAppId, "applications"), Convert.ToInt32(node.InnerText));
                    }
                }
                DbMethods.DeleteFromId("applications", id);
                return Ok("Application " + id + " deleted successfully");
            }
            return Content(HttpStatusCode.BadRequest, "Application " + id + " doesn't exist");
        }

        // !!!!!!!!!!!!!!!
        // !!! Modules !!!
        // !!!!!!!!!!!!!!!

        // Create
        [Route("{application}")]
        public IHttpActionResult PostModule(string application)
        {
            string sqlQueryAppIdFromName = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int applicationId = DbMethods.GetId(sqlQueryAppIdFromName, "applications");

            if (applicationId != 0)
            {
                GetXmlFromStream();

                HandlerXML handler = new HandlerXML(RECEIVED_FILE_PATH, XSD_PATH + "moduleVerification.xsd");

                if (handler.ValidateXML())
                {
                    XmlDocument rcvDoc = new XmlDocument();
                    rcvDoc.Load(RECEIVED_FILE_PATH);
                    XmlElement element = rcvDoc.DocumentElement;

                    if (element.GetAttribute("res_type") == "module")
                    {
                        int parentId = Convert.ToInt32(rcvDoc.SelectSingleNode("//parent").InnerText);
                        if (applicationId == parentId)
                        {
                            int moduleId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);

                            string sqlQuery = "SELECT * FROM modules WHERE id = " + moduleId + " ORDER BY id";
                            if (DbMethods.GetId(sqlQuery, "modules") != moduleId)
                            {
                                string moduleName = rcvDoc.SelectSingleNode("//name").InnerText;

                                string sqlString = "INSERT INTO modules values(@id, @name, @creation_dt, @parent)";

                                SqlCommand sqlCommand = new SqlCommand(sqlString);
                                sqlCommand.Parameters.AddWithValue("@id", moduleId);
                                sqlCommand.Parameters.AddWithValue("@name", moduleName);
                                sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                                sqlCommand.Parameters.AddWithValue("@parent", parentId);

                                DbMethods.ExecuteSqlCommand(sqlCommand);

                                return Ok("Module " + moduleName + " created successfully");
                            }
                            return Content(HttpStatusCode.BadRequest, "An module with id " + moduleId + " already exists");
                        }
                        return Content(HttpStatusCode.BadRequest, "Mismatch between URL App field and XML parent field");
                    }
                    return Content(HttpStatusCode.BadRequest, RES_TYPE_ERROR + "module ?");
                }
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }
            return Content(HttpStatusCode.BadRequest, "Application " + application + " doesn't exist");
        }

        // Read Modules
        [Route("{application}")]
        public IHttpActionResult GetModules(string application)
        {
            string sqlQueryAppIdFromName = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int applicationId = DbMethods.GetId(sqlQueryAppIdFromName, "applications");

            if (applicationId != 0)
            {
                string sqlQuery = "SELECT * FROM modules WHERE parent = " + applicationId + " ORDER BY id";
                XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "modules");

                if (doc.SelectSingleNode("//module") == null)
                {
                    return Content(HttpStatusCode.BadRequest, "No modules available");
                }
                return Ok(doc.OuterXml);
            }
            return Content(HttpStatusCode.BadRequest, "Application " + application + " doesn't exist");
        }

        // Read Module by id
        [Route("{application}/{id:int}")]
        public IHttpActionResult GetModuleById(string application, int id)
        {
            string sqlQueryAppIdFromName = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int parentId = DbMethods.GetId(sqlQueryAppIdFromName, "applications");

            if (parentId != 0)
            {
                string sqlQueryModuleId = "SELECT * FROM modules WHERE id = " + id + " AND parent = " + parentId + " ORDER BY id";
                if (DbMethods.GetId(sqlQueryModuleId, "modules") == id)
                {
                    XmlDocument doc = XmlUtils.GetSomething(sqlQueryModuleId, "modules");

                    return Ok(doc.OuterXml);
                }
                return Content(HttpStatusCode.BadRequest, "Module " + id + "doesn't exist on " + application);
            }
            return Content(HttpStatusCode.BadRequest, "Application " + application + " doesn't exist");
        }

        // Update
        [Route("{application}/{id:int}")]
        public IHttpActionResult PutModule(string application, int id)
        {
            string sqlQueryAppIdFromName = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int appId = DbMethods.GetId(sqlQueryAppIdFromName, "applications");

            if (appId != 0)
            {
                GetXmlFromStream();

                HandlerXML handler = new HandlerXML(RECEIVED_FILE_PATH, XSD_PATH + "moduleVerification.xsd");

                if (handler.ValidateXML())
                {
                    XmlDocument rcvDoc = new XmlDocument();
                    rcvDoc.Load(RECEIVED_FILE_PATH);
                    XmlElement element = rcvDoc.DocumentElement;

                    if (element.GetAttribute("res_type") == "module")
                    {
                        int parentId = Convert.ToInt32(rcvDoc.SelectSingleNode("//parent").InnerText);

                        //string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
                        if (appId == parentId)
                        {
                            string sqlQuery = "SELECT * FROM modules WHERE id = " + id + " AND parent = " + appId + " ORDER BY id";
                            XmlDocument doc = XmlUtils.GetSomething(sqlQuery, "applications");

                            Module oldModule = new Module();
                            oldModule.name = doc.SelectSingleNode("//name").InnerText;
                            string newName = rcvDoc.SelectSingleNode("//name").InnerText;

                            if (newName != oldModule.name)
                            {
                                string sqlString = "UPDATE modules SET name = \'" + newName + "\' WHERE id = " + id;

                                SqlCommand sqlCommand = new SqlCommand(sqlString);

                                DbMethods.ExecuteSqlCommand(sqlCommand);

                                return Ok("Module " + id + " updated successfully");
                            }
                            return Content(HttpStatusCode.BadRequest, "Module " + id + " not updated");
                        }
                        return Content(HttpStatusCode.BadRequest, application + " does not exist");
                    }
                    return Content(HttpStatusCode.BadRequest, RES_TYPE_ERROR + "module?");
                }
                return Content(HttpStatusCode.BadRequest, handler.ValidationMessage);
            }
            return Content(HttpStatusCode.BadRequest, "Application " + application + " doesn't exist");
        }

        // Delete
        [Route("{application}/{id:int}")]
        public IHttpActionResult DeleteModule(string application, int id)
        {
                string sqlQueryAppId = "SELECT * FROM applications WHERE name = \'" + application + "\'";
                int idApplication = DbMethods.GetId(sqlQueryAppId, "applications");

                if (idApplication != 0)
                {
                    string sqlQueryModuleId = "SELECT * FROM modules WHERE id = " + id + " AND parent = " + idApplication + " ORDER BY id";
                    if (DbMethods.GetId(sqlQueryModuleId, "modules") == id)
                    {
                        DbMethods.DeleteFromParent("subcriptions",id); // Delete the subscriptions of this module
                        DbMethods.DeleteFromParent("data",id); // Delete the data of this module
                        DbMethods.DeleteFromId("modules", id); // Delete the module
                        return Ok("Module " + id + " deleted successfully");
                    }
                    return Content(HttpStatusCode.BadRequest, "Module " + id + "does not exist on " + application); 
                }
                return Content(HttpStatusCode.BadRequest, application + " does not exist");
        }

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // !!! Data & Subscriptions !!!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // Create
        [Route("{application}/{module}")]
        // value can be Subscription or Data
        public IHttpActionResult PostSubModule(string application, string module)
        {
            string sqlQueryAppIdFromName = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int applicationId = DbMethods.GetId(sqlQueryAppIdFromName, "applications");

            if (applicationId != 0)
            {
                string sqlQueryModuleIdFromName = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + applicationId;
                int moduleId = DbMethods.GetId(sqlQueryModuleIdFromName, "applications");

                if (moduleId != 0)
                {
                    GetXmlFromStream();

                    HandlerXML dataHandler = new HandlerXML(RECEIVED_FILE_PATH, XSD_PATH + "dataVerification.xsd");
                    HandlerXML subscriptionHandler = new HandlerXML(RECEIVED_FILE_PATH, XSD_PATH + "subscriptionVerification.xsd");

                    if (dataHandler.ValidateXML() || subscriptionHandler.ValidateXML())
                    {
                        XmlDocument rcvDoc = new XmlDocument();
                        rcvDoc.Load(RECEIVED_FILE_PATH);
                        XmlElement element = rcvDoc.DocumentElement;

                        int parentId = Convert.ToInt32(rcvDoc.SelectSingleNode("//parent").InnerText);
                        if (parentId == moduleId)
                        {
                            if (element.GetAttribute("res_type") == "data")
                            {
                                int dataId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);

                                string sqlQuery = "SELECT * FROM data WHERE id = " + dataId + " ORDER BY id";
                                if (DbMethods.GetId(sqlQuery, "data") != dataId)
                                {
                                    string dataContent = rcvDoc.SelectSingleNode("//content").InnerText;

                                    string sqlString = "INSERT INTO data values(@id, @content, @creation_dt, @parent)";

                                    SqlCommand sqlCommand = new SqlCommand(sqlString);
                                    sqlCommand.Parameters.AddWithValue("@id", dataId);
                                    sqlCommand.Parameters.AddWithValue("@content", dataContent);
                                    sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                                    sqlCommand.Parameters.AddWithValue("@parent", parentId);

                                    System.Diagnostics.Debug.WriteLine(sqlString);
                                    DbMethods.ExecuteSqlCommand(sqlCommand);

                                    return Ok("Data created successfully");
                                }
                                return Content(HttpStatusCode.BadRequest, "Data " + dataId + " already exists");
                            }
                            else if (element.GetAttribute("res_type") == "subscription")
                            {
                                int subcriptionId = Convert.ToInt32(rcvDoc.SelectSingleNode("//id").InnerText);

                                string sqlQuery = "SELECT * FROM subcriptions WHERE id = " + subcriptionId + " ORDER BY id";
                                if (DbMethods.GetId(sqlQuery, "subcriptions") != subcriptionId)
                                {
                                    string subscriptionName = rcvDoc.SelectSingleNode("//name").InnerText;
                                    string subscriptionEvent = rcvDoc.SelectSingleNode("//event").InnerText;
                                    string subscriptionEnpoint = rcvDoc.SelectSingleNode("//endpoint").InnerText;

                                    string sqlString = "INSERT INTO subscriptions values(@id, @name, @creation_dt, @parent, @event, @endpoint)";

                                    SqlCommand sqlCommand = new SqlCommand(sqlString);
                                    sqlCommand.Parameters.AddWithValue("@id", subcriptionId);
                                    sqlCommand.Parameters.AddWithValue("@name", subscriptionName);
                                    sqlCommand.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                                    sqlCommand.Parameters.AddWithValue("@parent", parentId);
                                    sqlCommand.Parameters.AddWithValue("@event", subscriptionEvent);
                                    sqlCommand.Parameters.AddWithValue("@endpoint", subscriptionEnpoint);

                                    System.Diagnostics.Debug.WriteLine(sqlString);
                                    DbMethods.ExecuteSqlCommand(sqlCommand);

                                    return Ok("Subscription created successfully");
                                }
                                return Content(HttpStatusCode.BadRequest, "Subcription " + subcriptionId + " already exists");
                            }
                            return Content(HttpStatusCode.BadRequest, RES_TYPE_ERROR + " do you mean data or subscription ?");
                        }
                        return Content(HttpStatusCode.BadRequest, "parent = " + parentId + " != module = " + moduleId);
                    }
                    return Content(HttpStatusCode.BadRequest, "Provided XML is Malformed");
                }
                return Content(HttpStatusCode.BadRequest, module + " does not exist within " + application);
            }
            return Content(HttpStatusCode.BadRequest, application + " does not exist");
        }

        // Delete
        //[Route("{application}/{module}/{id:int}")]
        [Route("{application}/{module}/{type}/{id:int}")]
        //public IHttpActionResult DeleteSubModule(string application, string module, int id)
        public IHttpActionResult DeleteSubModule(string application, string module, string type, int id)
        {
            string sqlQueryAppIdFromName = "SELECT * FROM applications WHERE name = \'" + application + "\'";
            int applicationId = DbMethods.GetId(sqlQueryAppIdFromName, "applications");

            if (applicationId != 0)
            {
                string sqlQueryModuleIdFromName = "SELECT * FROM modules WHERE name = \'" + module + "\' AND parent = " + applicationId;
                int moduleId = DbMethods.GetId(sqlQueryModuleIdFromName, "applications");

                if (moduleId != 0)
                {
                    if (type == "data")
                    {
                        DbMethods.DeleteFromId("data", id);
                        return Ok("Data " + id + " deleted successfully");
                    }
                    else if (type == "subscription")
                    {
                        DbMethods.DeleteFromId("subscriptions", id);
                        return Ok("Subscription " + id + " deleted successfully");
                    }
                }
                return Content(HttpStatusCode.BadRequest, module + " does not exist within " + application);
            }
            return Content(HttpStatusCode.BadRequest, application + " does not exist");
        }


        // !!!!!!!!!!!!!!!!!!!!!!!
        // !! General Functions !!
        // !!!!!!!!!!!!!!!!!!!!!!!

        public static void GetXmlFromStream()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();

            StreamWriter writer = new StreamWriter(RECEIVED_FILE_PATH);
            writer.Write(bodyText);
            writer.Close();
        }

    }
}
