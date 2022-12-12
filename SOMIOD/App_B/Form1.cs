using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace App_B
{
    public partial class Form1 : Form
    {
        string app = "AppA";
        string module = "ModuleA";
        public Form1()
        {
            InitializeComponent();
        }

        private void onButton_Click(object sender, EventArgs e)
        {
            string url = "http://localhost:50768//api/somiod/"+app+"/"+module;
            string xml = "<data res_type=\"data\">\r\n    <id>7</id>\r\n    <content>ON</content>\r\n    <parent>1</parent>\r\n</data>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(xml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                MessageBox.Show(responseStr);
            }
        }
    }
}
