using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;



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
    public class mostPlayed
    {
        public long watcher_count { get; set; }
        public long play_count { get; set; }
        public long collected_count { get; set; }
        public Movie movie { get; set; }
    }
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                String moviess = "";
                String noMoviess = "";
                int count = 0;
                using (var client = new HttpClient { BaseAddress = new Uri("https://api.trakt.tv/") })
                {
                    client.DefaultRequestHeaders.Add("trakt.api.key", "468a92c26d3411be7886881b7f40afea47288963a91d9c5a0f43257521ceab74");
                    using (var response = client.GetAsync("movies/popular").Result)
                    {
                        var responses = client.GetAsync("movies/played").Result;

                        //This one is for the popular movies
                        var responseString = response.Content.ReadAsStringAsync().Result;

                        //This one is for the Most Played movies
                        var responseMostPlayedString = responses.Content.ReadAsStringAsync().Result;

                        // List<Movie> movies = new List<Movie>();
                        var responseJSON = JsonConvert.DeserializeObject<List<Movie>>(responseString);
                        var responseMostPlayedJSON = JsonConvert.DeserializeObject<List<mostPlayed>>(responseMostPlayedString);
                        if (message.Text.ToLower().Contains("popular") && message.Text.ToLower().Contains("most played"))
                        {
                            for (int i = 0; i < responseJSON.Count; i++)
                            {
                                if (!responseJSON[i].title.ToString().ToLower().Contains(message.Text.ToLower()))
                                {
                                    count++;
                                    if (i == 0)
                                    {
                                        moviess += $"{Environment.NewLine}{Environment.NewLine}------Popular Movies-------";
                                    }
                                    moviess += $"{Environment.NewLine}{Environment.NewLine} " + count + ")" + responseJSON[i].title.ToString();
                                }
                            }
                            //Most played
                            for (int i = 0; i < responseMostPlayedJSON.Count; i++)
                            {
                                if (!responseMostPlayedJSON[i].movie.title.ToString().ToLower().Contains(message.Text.ToLower()))
                                {
                                    count++;
                                    if (i == 0)
                                    {
                                        moviess += $"{Environment.NewLine}{Environment.NewLine}------Most Played Movies-------";
                                    }
                                    moviess += $"{Environment.NewLine}{Environment.NewLine}  " + count + ")" + responseMostPlayedJSON[i].movie.title.ToString();
                                }
                            }
                        }
                        else if (message.Text.ToLower().Contains("popular"))
                        {
                            for (int i = 0; i < responseJSON.Count; i++)
                            {
                                if (responseJSON[i].title.ToString().ToLower().Contains(message.Text.ToLower()) && message.Text.ToLower().Contains("popular"))
                                {
                                    count++;
                                    moviess += $"{Environment.NewLine}{Environment.NewLine}" + count + ")" + responseJSON[i].title.ToString();
                                }
                                else if (message.Text.ToLower().Contains("popular"))
                                {
                                    count++;
                                    moviess += $"{Environment.NewLine}{Environment.NewLine} " + count + ")" + responseJSON[i].title.ToString();
                                }
                            }
                        }
                        else if (message.Text.ToLower().Contains("most play"))
                        {
                            for (int i = 0; i < responseMostPlayedJSON.Count; i++)
                            {
                                if (responseJSON[i].title.ToString().ToLower().Contains(message.Text.ToLower()) && message.Text.ToLower().Contains("most played"))
                                {
                                    count++;
                                    moviess += $"{Environment.NewLine}{Environment.NewLine}" + count + ")" + responseMostPlayedJSON[i].movie.title.ToString();
                                }
                                else if (message.Text.ToLower().Contains("most played"))
                                {
                                    count++;
                                    moviess += $"{Environment.NewLine}{Environment.NewLine} " + count + ")" + responseMostPlayedJSON[i].movie.title.ToString();
                                }
                            }

                        }
                        if (!(message.Text.ToLower().Contains("popular") && message.Text.ToLower().Contains("most played")))
                        {
                            for (int i = 0; i < responseJSON.Count; i++)
                            {
                                if (responseJSON[i].title.ToString().ToLower().Contains(message.Text.ToLower()))
                                {
                                    count++;
                                    if (i == 0)
                                    {
                                        moviess += $"{Environment.NewLine}{Environment.NewLine}------Popular Movies-------";
                                    }
                                    moviess += $"{Environment.NewLine}{Environment.NewLine} " + count + ")" + responseJSON[i].title.ToString();
                                }
                            }
                            //Most played
                            int countForMostPlayed = 0;
                            for (int i = 0; i < responseMostPlayedJSON.Count; i++)
                            {
                                if (responseMostPlayedJSON[i].movie.title.ToString().ToLower().Contains(message.Text.ToLower()))
                                {
                                    count++;
                                    if( countForMostPlayed == 0)
                                    {
                                        moviess += $"{Environment.NewLine}{Environment.NewLine}------Most Played Movies-------";
                                    }
                                    countForMostPlayed++;
                                   moviess += $"{Environment.NewLine}{Environment.NewLine}  " + count + ")" + responseMostPlayedJSON[i].movie.title.ToString();
                                }
                            }
                        }
                        if (count < 1)
                            {

                                for (int j = 0; j < responseJSON.Count; j++)
                                {
                                    noMoviess += $"{Environment.NewLine}{Environment.NewLine} " + (j+1) + ")" + responseJSON[j].title.ToString();
                                }

                            }
                        }
                    }
                    if (count < 1)
                    {
                        return message.CreateReplyMessage($"Nothing found That matches you serach. Here is a list of all Populer Movies that you might want to watch:" + noMoviess);
                    }
                    else
                    {
                        return message.CreateReplyMessage($"The list of Movies You looking for:" + moviess);
                    }

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