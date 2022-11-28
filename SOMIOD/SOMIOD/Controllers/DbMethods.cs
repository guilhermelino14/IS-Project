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


        public static void DeleteSomething(string location, int id)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(MainController.connectionString);
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

        public static int VerifyOnDB(string sqlQuery, string type)
        {
            XmlDocument doc = XmlUtils.GetSomething(sqlQuery, type);
            System.Diagnostics.Debug.WriteLine(doc.SelectSingleNode("//id"));

            if (doc.SelectSingleNode("//id") == null)
            {
                return 0;
            }

            int id = Convert.ToInt32(doc.SelectSingleNode("//id").InnerText);
            System.Diagnostics.Debug.WriteLine(id);
            return id;
        }
    }
}