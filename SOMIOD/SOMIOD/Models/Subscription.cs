using System;

namespace SOMIOD.Models
{
    public class Subscription
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime creation_dt { get; set; }
        public int parent { get; set; }
        public string subscription_event { get; set; }
        public string endpoint { get; set; }
    }
}