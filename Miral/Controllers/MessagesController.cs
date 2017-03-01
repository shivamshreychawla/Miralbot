using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using static Miral.miralclasses;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Collections.Generic;
using M = Microsoft.Bot.Connector;

namespace Miral
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public string Get()
        {
            return "Shivam";
        }

        private static async Task<MiralLUIS> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            MiralLUIS Data = new MiralLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/fc19f533-1a4e-48e0-9911-accdd4e63531?subscription-key=d45e14dfb4264ee88305922abb9b9f46&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<MiralLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            string miralLuisoutput = "empty";
            Activity replyToConversation = activity.CreateReply();
            replyToConversation.Attachments = new List<Attachment>();
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    int length = (activity.Text ?? string.Empty).Length;
                    MiralLUIS StLUIS = await GetEntityFromLUIS(activity.Text);

                    if (StLUIS.topScoringIntent != null && !string.IsNullOrEmpty(StLUIS.topScoringIntent.intent))
                    {
                        switch (StLUIS.topScoringIntent.intent)
                        {
                            case "findrides":
                                //miralLuisoutput = $"Folowing ride are available (every 30 mins): Splash, RollerCoaster, Dragon, MiralEye, Transformers and RoundRobin";
                                miralLuisoutput = "";
                                CardAction Splash = new CardAction()
                                {
                                    Type = "imBack",
                                    Title = "Splash",
                                    Value = "Show Splash timings"
                                };
                                CardAction RollerCoaster = new CardAction()
                                {
                                    Type = "imBack",
                                    Title = "RollerCoaster",
                                    Value = "Show RollerCoaster timings"
                                };
                                CardAction Dragon = new CardAction()
                                {
                                    Type = "imBack",
                                    Title = "Dragon",
                                    Value = "Show Dragon timings"
                                };
                                CardAction Transformers = new CardAction()
                                {
                                    Type = "imBack",
                                    Title = "Transformers",
                                    Value = "Show Transformers timings"
                                };

                                List<CardAction> cardButtons = new List<CardAction>();
                                cardButtons.Add(Splash);
                                cardButtons.Add(RollerCoaster);
                                cardButtons.Add(Dragon);
                                cardButtons.Add(Transformers);
                                HeroCard plCard = new HeroCard()
                                {
                                    Title = "Folowing ride are available:",
                                    Subtitle = " (every 30 mins)",
                                    Buttons = cardButtons
                                };
                                Attachment plAttachment = plCard.ToAttachment();
                                replyToConversation.Attachments.Add(plAttachment);
                                break;
                            case "None":
                                miralLuisoutput = "None Intent";
                                break;
                            case "greeting":
                                miralLuisoutput = "Hello, Welcome to Miral Theme Park, Have a Good Day :)";
                                Attachment att = new Attachment()
                                {
                                    ContentUrl = "https://1a7ae9d8.ngrok.io/miral/miralthemepark.jpg",
                                    ContentType = "image/jpg",
                                };
                                replyToConversation.Attachments.Add(att);

                                break;
                            case "ridetimes":
                                if (StLUIS.entities != null && StLUIS.entities.Count() > 0 && !string.IsNullOrEmpty(StLUIS.entities[0].entity))
                                {
                                    Random r = new Random();
                                    int x = r.Next(10, 20);//Max range
                                    miralLuisoutput = $"Next ride time for {StLUIS.entities[0].entity} is at {x}:00  PM, enjoy your ride :)";
                                    Attachment att1 = new Attachment()
                                    {
                                        ContentUrl = $"https://1a7ae9d8.ngrok.io/miral/" + StLUIS.entities[0].entity + ".jpg",
                                        ContentType = "image/jpg",
                                    };
                                    replyToConversation.Attachments.Add(att1);
                                }
                                break;
                            case "weather":
                                HttpClient client = new HttpClient();
                                var rsult = client.GetAsync("http://api.openweathermap.org/data/2.5/weather?q=Delhi,india&appid=eac3f002d569916e04d2ed17c3f457d0").Result;
                                var contents = rsult.Content.ReadAsStringAsync().Result;
                                var res = new JavaScriptSerializer().Deserialize<WeatherObjectRoot>(contents);

                                miralLuisoutput = $"Temprature in Delhi is : {(res.main.temp - 273)} °C";
                                break;
                            default:
                                miralLuisoutput = "Sorry, I am not getting you...";
                                break;
                        }
                    }
                }
                else
                {
                    HandleSystemMessage(activity);
                }
            }
            catch (Exception ex)
            {
                miralLuisoutput = $"Exception = {ex.Message}";
            }
            replyToConversation.Text = miralLuisoutput;
            await connector.Conversations.SendToConversationAsync(replyToConversation);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }



        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}