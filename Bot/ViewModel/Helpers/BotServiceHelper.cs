using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bot.ViewModel.Helpers
{
    public class BotServiceHelper
    {
        private string _baseUrl;
        private string _endpoint;
        private string _secretKey;

        public Conversation _Conversation { get; set; }
        public BotServiceHelper()
        {
            Run();
        }

        private async void CreateConversation()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_secretKey}");

                var response = await client.PostAsync(_endpoint, null);
                string json = await response.Content.ReadAsStringAsync();

                _Conversation = JsonConvert.DeserializeObject<Conversation>(json);
            }
        }

        public async void SendActivity(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_secretKey}");

                Activity activity = new Activity
                {
                    From = new ChannelAccount
                    {
                        Id = "user1"
                    },
                    Text = message,
                    Type = "message"
                };

                string jsonContent = JsonConvert.SerializeObject(activity);
                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync($"{_endpoint}/{_Conversation.ConversationId}/activities", byteContent);
                string json = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(json);
                string id = (string)obj.SelectToken("id");
            }
        }

        private async Task Configure()
        {
            using (var cts = new CancellationTokenSource())
            {
                var config = await ConfigurationManager.Instance.GetAsync(cts.Token);
                _baseUrl = config.ConversationBaseURL;
                _endpoint = config.ConversationEndpoint;
                _secretKey = config.WebChatSecretKey;
            }
        }

        private async void Run()
        {
            await Configure();
            CreateConversation();
        }

        public class ChannelAccount
        {
            public string Id 
            { 
                get; 
                set; 
            }

            public string Name
            {
                get;
                set;
            }
        }

        public class Activity
        {
            public ChannelAccount From
            {
                get;
                set;
            }

            public string Text
            {
                get;
                set;
            }

            public string Type
            {
                get;
                set;
            }
        }

        public class Conversation
        {
            public string ConversationId
            {
                get;
                set;
            }

            public string Token
            {
                get;
                set;
            }

            public string StreamUrl
            {
                get;
                set;
            }

            public string ReferenceGrammarId
            {
                get;
                set;
            }

            public int Expires_in
            {
                get;
                set;
            }
        }
    }
}
