using System;
using System.Net.Http;
using System.Threading;
using Bot.Configuration;
using Newtonsoft.Json;

namespace Bot.ViewModel.Helpers
{
    public class BotServiceHelper
    {
        public Conversation _Conversation { get; set; }
        public BotServiceHelper()
        {
            CreateConversation();
        }

        private async void CreateConversation()
        {
            string baseUrl;
            string endpoint;
            string secretKey;

            using (var cts = new CancellationTokenSource())
            {
                var config = await ConfigurationManager.Instance.GetAsync(cts.Token);
                baseUrl = config.ConversationBaseURL;
                endpoint = config.ConversationEndpoint;
                secretKey = config.WebChatSecretKey;
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

                var response = await client.PostAsync(endpoint, null);
                string json = await response.Content.ReadAsStringAsync();

                _Conversation = JsonConvert.DeserializeObject<Conversation>(json);
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
