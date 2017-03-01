using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Miral
{
    public class OrderHandler
    {
        public List<OrderReturnRootobject> AllOrders(string orderType)
        {
            string fileName = string.Empty;
            if (orderType == "ordersPending")
                fileName = @"~/data.txt";
            else
                fileName = @"~/dataCompleted.txt";

            return new JavaScriptSerializer().Deserialize<List<OrderReturnRootobject>>
                (System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(fileName)));
        }

        public void WriteOrder(List<OrderReturnRootobject> orders, string orderType)
        {
            string fileName = string.Empty;
            if (orderType == "ordersPending")
                fileName = @"~/data.txt";
            else
                fileName = @"~/dataCompleted.txt";

            var json = new JavaScriptSerializer().Serialize(orders);
            using (StreamWriter w = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath(fileName)))
            {
                w.WriteLine(json);
            }


        }


        public string PriceDetails(string vegetable)
        {
            if (vegetable.ToLower().Contains("onion"))
                return "25";
            else if (vegetable.ToLower().Contains("tomato"))
                return "30";
            else if (vegetable.ToLower().Contains("potato"))
                return "20";
            else
                return "10";
        }

    }

    public class OrderReturnRootobject
    {
        public string OrderId { get; set; }
        public string emailId { get; set; }
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
        public string Price { get; set; }
        public string DateTime { get; set; }
        public string Address { get; set; }
        public string currency { get; set; }
    }

    public class OrderRootobject
    {
        public string id { get; set; }
        public DateTime? timestamp { get; set; }
        public Result result { get; set; }
        public Status status { get; set; }
        public string sessionId { get; set; }
    }

    public class Result
    {
        public string source { get; set; }
        public string resolvedQuery { get; set; }
        public string action { get; set; }
        public bool actionIncomplete { get; set; }
        public Parameters parameters { get; set; }
        public object[] contexts { get; set; }
        public Metadata metadata { get; set; }
        public Fulfillment fulfillment { get; set; }
        public int score { get; set; }
    }

    public class Parameters
    {
        public string Address { get; set; }
        public DateTime[] date { get; set; }
        public string email { get; set; }
        [JsonProperty(PropertyName = "unit-weight")]
        public UnitWeight unitweight { get; set; }
        public string vegetables { get; set; }
    }

    public class UnitWeight
    {
        public int amount { get; set; }
        public string unit { get; set; }
    }

    public class Metadata
    {
        public string intentId { get; set; }
        public string webhookUsed { get; set; }
        public string webhookForSlotFillingUsed { get; set; }
        public string intentName { get; set; }
    }

    public class Fulfillment
    {
        public string speech { get; set; }
        public Message[] messages { get; set; }
    }

    public class Message
    {
        public int type { get; set; }
        public string speech { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public Facebook facebook { get; set; }
        public Kik kik { get; set; }
        public Slack slack { get; set; }
        public Telegram telegram { get; set; }
    }

    public class Facebook
    {
        public Attachment1 attachment { get; set; }
    }

    public class Attachment1
    {
        public string type { get; set; }
        public Payload1 payload { get; set; }
    }

    public class Payload1
    {
    }

    public class Kik
    {
        public string type { get; set; }
        public string body { get; set; }
    }

    public class Slack
    {
        public string text { get; set; }
        public object[] attachments { get; set; }
    }

    public class Telegram
    {
        public string text { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string errorType { get; set; }
    }


    public class Rootobject
    {
        public string speech { get; set; }
        public string displayText { get; set; }
        public string data { get; set; }
        public string contextOut { get; set; }
        public string source { get; set; }
    }


    public class WeatherObjectRoot
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public float visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public float dt { get; set; }
        public Sys sys { get; set; }
        public float id { get; set; }
        public string name { get; set; }
        public float cod { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float pressure { get; set; }
        public float humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public float deg { get; set; }
    }

    public class Clouds
    {
        public float all { get; set; }
    }

    public class Sys
    {
        public float type { get; set; }
        public float id { get; set; }
        public float message { get; set; }
        public string country { get; set; }
        public float sunrise { get; set; }
        public float sunset { get; set; }
    }

    public class Weather
    {
        public float id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

}