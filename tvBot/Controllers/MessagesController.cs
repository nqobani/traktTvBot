using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;


using System.Security.Policy;
using System.Net.Http.Headers;

namespace tvBot
{
    //[BotAuthentication]
    public class Ids
    {
        public long trakt { get; set; }
        public string slug { get; set; }
        public string imdb { get; set; }
        public long tmdb { get; set; }
    }

    public class Movie
    {
        public string title { get; set; }
        public long year { get; set; }
        public Ids ids { get; set; }

        public override string ToString()
        {
            return $"{title}-{year}";
        }
    }
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                String moviess = "";
                using (var client = new HttpClient { BaseAddress = new Uri("https://api.trakt.tv/") })
                {
                    client.DefaultRequestHeaders.Add("trakt.api.key", "468a92c26d3411be7886881b7f40afea47288963a91d9c5a0f43257521ceab74");
                    using (var response = client.GetAsync("movies/popular").Result)
                    {
                        var responseString = response.Content.ReadAsStringAsync().Result;

                        // List<Movie> movies = new List<Movie>();
                        var responseJSON = JsonConvert.DeserializeObject<List<Movie>>(responseString);

                        for (int i = 0; i < responseJSON.Count; i++)
                        {
                            if (responseJSON[i].title.ToString().ToLower().Contains(message.Text.ToLower()))
                            {
                                moviess += "  --------------------------------------   " + responseJSON[i].title.ToString();
                            }

                        }
                    }
                }
                return message.CreateReplyMessage($"The list of Populer Movies:" + moviess);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}