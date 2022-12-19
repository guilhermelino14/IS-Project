using Newtonsoft.Json.Linq;
using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Application = SOMIOD.Models.Application;

namespace SOMIOD.Controllers
{
    public class XmlUtils
    {
        public static XmlDocument GetSomething(string sqlString, string type)
        {
            SqlConnection conn = null;

            XmlDocument doc = CreateXML();
            XmlElement root = doc.CreateElement(type);
            doc.AppendChild(root);

            try
            {
                conn = new SqlConnection(MainController.connectionString);
                conn.Open();
                SqlCommand command = new SqlCommand(sqlString, conn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    switch (type)
                    {
                        case ("applications"):
                            Application app = DbMethods.FillApplication(reader);
                            root.AppendChild(CreateApplication(doc, app.id, app.name, app.creation_dt, "application"));
                            break;
                        case ("modules"):
                            Module mod = DbMethods.FillModule(reader);
                            root.AppendChild(CreateModule(doc, mod.id, mod.name, mod.creation_dt, mod.parent, "module"));
                            break;
                        case ("data"):
                            Data data = DbMethods.FillData(reader);
                            root.AppendChild(CreateData(doc, data.id, data.content, data.creation_dt, data.parent, "data"));
                            break;
                        case ("subscriptions"):
                            Subscription subscription = DbMethods.FillSubscription(reader);
                            root.AppendChild(CreateSubscription(doc, subscription.id, subscription.name, subscription.creation_dt, subscription.parent, subscription.subscription_event, subscription.endpoint, "subscription"));
                            break;
                    }
                    doc.Save(MainController.RESPONSE_FILE_PATH);
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

        public static XmlDocument CreateXML()
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", null, null);
            xmlDocument.AppendChild(xmlDeclaration);

            return xmlDocument;
        }

        public static XmlElement CreateApplication(XmlDocument doc, int id, string name, string creation_dt, string type)
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

        public static XmlElement CreateModule(XmlDocument doc, int id, string name, string creation_dt, int parent, string type)
        {
            XmlElement module = CreateApplication(doc, id, name, creation_dt, type);

            XmlElement parentE = doc.CreateElement("parent");
            parentE.InnerText = parent.ToString();

            module.AppendChild(parentE);

            XmlDocument dataDoc = GetSomething("SELECT * FROM data WHERE parent = " + id, "data");
            XmlElement dataRootE = doc.CreateElement("data");

            foreach (XmlNode node in dataDoc.SelectNodes("/data//data"))
            {
                XmlElement dataE = doc.CreateElement("data");
                XmlElement dataIdE = doc.CreateElement("id");
                dataIdE.InnerText = node.SelectSingleNode("id").InnerText;
                dataE.AppendChild(dataIdE);
                XmlElement dataContentE = doc.CreateElement("content");
                dataContentE.InnerText = node.SelectSingleNode("content").InnerText;
                dataE.AppendChild(dataContentE);
                XmlElement dataCreationDateE = doc.CreateElement("creation_dt");
                dataCreationDateE.InnerText = node.SelectSingleNode("creation_dt").InnerText;
                dataE.AppendChild(dataCreationDateE);
                dataRootE.AppendChild(dataE);
            }
            module.AppendChild(dataRootE);

            return module;
        }

        public static XmlElement CreateData(XmlDocument doc, int id, string content, string creation_dt, int parent, string type)
        {
            XmlElement data = doc.CreateElement(type);

            XmlElement idE = doc.CreateElement("id");
            idE.InnerText = id.ToString();
            XmlElement contentE = doc.CreateElement("content");
            contentE.InnerText = content;
            XmlElement creation_dtE = doc.CreateElement("creation_dt");
            creation_dtE.InnerText = creation_dt;

            data.AppendChild(idE);
            data.AppendChild(contentE);
            data.AppendChild(creation_dtE);

            return data;
        }

        public static XmlElement CreateSubscription(XmlDocument doc, int id, string name, string creation_dt, int parent, string sub_event, string endpoint, string type)
        {
            XmlElement subscription = CreateApplication(doc, id, name, creation_dt, type);

            XmlElement parentE = doc.CreateElement("parent");
            parentE.InnerText = parent.ToString();
            XmlElement creation_dtE = doc.CreateElement("creation_dt");
            creation_dtE.InnerText = creation_dt;
            XmlElement eventE = doc.CreateElement("event");
            eventE.InnerText = sub_event;
            XmlElement endpointE = doc.CreateElement("endpoint");
            endpointE.InnerText = endpoint;

            subscription.AppendChild(parentE);
            subscription.AppendChild(creation_dtE);
            subscription.AppendChild(eventE);
            subscription.AppendChild(endpointE);

            return subscription;
        }

    }
}