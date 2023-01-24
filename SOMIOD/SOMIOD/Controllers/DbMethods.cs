using SOMIOD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml;

namespace SOMIOD.Controllers
{
    public class DbMethods
    {
        public static void ExecuteSqlCommand(SqlCommand sqlCommand)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(MainController.connectionString);
                sqlConnection.Open();

                sqlCommand.Connection = sqlConnection;
                SqlCommand command = sqlCommand;

                int nrows = command.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine(nrows.ToString());
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

        public static void DeleteFromId(string location, int id)
        {
            string sqlString = "DELETE FROM " + location + " WHERE id = " + id;
            DeleteSomething(sqlString);
        }

        public static void DeleteFromParent(string location, int parentId)
        {
            string sqlString = "DELETE FROM " + location + " WHERE parent = " + parentId;
            DeleteSomething(sqlString);
        }

        public static void DeleteSomething(string sqlString)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(MainController.connectionString);
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sqlString, sqlConnection);

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

        public static Application FillApplication(SqlDataReader reader)
        {
            Application application = new Application
            {
                id = (int)reader["id"],
                name = (string)reader["name"],
                creation_dt = (string)reader["creation_dt"],
            };
            return application;
        }

        public static Module FillModule(SqlDataReader reader)
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

        public static Data FillData(SqlDataReader reader)
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

        public static Subscription FillSubscription(SqlDataReader reader)
        {
            Subscription subscription = new Subscription
            {
                id = (int)reader["id"],
                name = (string)reader["name"],
                creation_dt = (string)reader["creation_dt"],
                parent = (int)reader["parent"],
                subscription_event = (string)reader["event"],
                endpoint = (string)reader["endpoint"],
            };
            return subscription;
        }

        public static int GetId(string sqlQuery, string type)
        {
            XmlDocument doc = XmlUtils.GetSomething(sqlQuery, type);

            if (doc.SelectSingleNode("//id") == null)
            {
                return 0;
            }

            int id = Convert.ToInt32(doc.SelectSingleNode("//id").InnerText);
            return id;
        }

        public static string GetName(string sqlQuery, string type)
        {
            XmlDocument doc = XmlUtils.GetSomething(sqlQuery, type);

            if (doc.SelectSingleNode("//name") == null)
            {
                return null;
            }

            string name = doc.SelectSingleNode("//name").InnerText;
            return name;
        }

    }
}