using System;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
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

            ReadMessage();
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

        public async void ReadMessage()
        {
            var client = new ClientWebSocket();
            var cts = new CancellationTokenSource();

            await client.ConnectAsync(new Uri(_Conversation.StreamUrl), cts.Token);

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    WebSocketReceiveResult result;
                    var message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await client.ReceiveAsync(message, cts.Token);
                        if (result.MessageType != WebSocketMessageType.Text)
                            break;
                        var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                        string messageJSON = Encoding.UTF8.GetString(messageBytes);
                    }
                    while (!result.EndOfMessage);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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
