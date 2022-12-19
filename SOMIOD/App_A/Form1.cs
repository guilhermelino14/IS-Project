using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace App_A
{
    public partial class Form1 : Form
    {
        MqttClient mClient = new MqttClient(IPAddress.Parse("127.0.0.1")); //OR use the broker hostname
        string[] mStrTopicsInfo = { "news" };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

        }
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //MessageBox.Show("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " +
            //e.Topic);
            
            stateLabel.BeginInvoke((MethodInvoker)delegate
            {
                stateLabel.Text = (Encoding.UTF8.GetString(e.Message));
                stateLabel.Font = new Font("Arial", 20);
            });
            
        }

        private void OnButtonPublish_Click()
        {
            if (mClient.IsConnected)
            {
                mClient.Publish(textBox1.Text, Encoding.UTF8.GetBytes("Hello World!"));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mClient.IsConnected)
            {
                mClient.Unsubscribe(mStrTopicsInfo); //Put this in a button to see notif!
                mClient.Disconnect(); //Free process and process's resources
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            mClient.Connect(Guid.NewGuid().ToString());
            if (!mClient.IsConnected)
            {
                MessageBox.Show("Error connecting to message broker...");
                label1.ForeColor = Color.Red;
                return;
            }
            label1.ForeColor = Color.Green;
            label1.Text = "Connected";
            
            mStrTopicsInfo[0] = textBox1.Text;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };//QoS
            mClient.Subscribe(mStrTopicsInfo, qosLevels);
            mClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            
        }
    }
}
