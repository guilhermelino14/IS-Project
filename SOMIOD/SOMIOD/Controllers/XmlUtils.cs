using Newtonsoft.Json.Linq;
using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

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
                        case ("aodules"):
                            Module mod = DbMethods.FillModule(reader);
                            root.AppendChild(CreateModule(doc, mod.id, mod.name, mod.creation_dt, mod.parent, "module"));
                            break;
                        case ("data"):
                            Data data = DbMethods.FillData(reader);
                            root.AppendChild(CreateData(doc, data.id, data.content, data.creation_dt, data.parent, "data"));
                            break;
                        case ("subscriptions"):
                            Subscription sub = DbMethods.FillSubscription(reader);
                            root.AppendChild(CreateSubscription(doc, sub.id, sub.name, sub.creation_dt, sub.parent, sub.subscription_event, sub.endpoint, "subscription"));
                            break;
                    }
                    doc.Save(MainController.GFILE_PATH);
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
            XmlElement parentE = doc.CreateElement("parent");
            parentE.InnerText = parent.ToString();

            data.AppendChild(idE);
            data.AppendChild(contentE);
            data.AppendChild(creation_dtE);
            data.AppendChild(parentE);

            return data;
        }

        public static XmlElement CreateSubscription(XmlDocument doc, int id, string name, string creation_dt, int parent, string eve, string endpoint, string type)
        {
            XmlElement subscription = CreateModule(doc, id, name, creation_dt, parent, type);

            XmlElement eventE = doc.CreateElement("event");
            eventE.InnerText = eve;
            XmlElement endpointE = doc.CreateElement("endpoint");
            endpointE.InnerText = endpoint;

            subscription.AppendChild(eventE);
            subscription.AppendChild(endpointE);

            return subscription;
        }



        
    }
}