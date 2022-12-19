using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SOMIOD.Controllers
{
    public class brokerUtils
    {
        public static MqttClient mClient = new MqttClient(IPAddress.Parse("127.0.0.1"));
        public static string[] mStrTopicsInfo = { "news", "complaints" };

        public static void connect()
        {
            mClient.Connect(Guid.NewGuid().ToString());
            if (!mClient.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("Error connecting to message broker...");
            }
            mClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }
        
        public static void disconnect()
        {
            mClient.Unsubscribe(mStrTopicsInfo);
            mClient.Disconnect();
        }

        public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);
        }

        public static void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("UNSUBSCRIBED WITH SUCCESS");
        }

        public static void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SUBSCRIBED WITH SUCCESS");
        }

        public static void postMessage(string topico, string message)
        {
            mClient.Publish(topico, Encoding.UTF8.GetBytes(message));
            
        }
    }
}