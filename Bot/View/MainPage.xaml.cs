using System.Threading;
using Bot.Configuration;
using Xamarin.Forms;

namespace Bot.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //using (var cts = new CancellationTokenSource())
            //{
            //    var config = await ConfigurationManager.Instance.GetAsync(cts.Token);
            //    webView.Source = $"{config.WebChatURL}?s={config.WebChatSecretKey}";
            //}
        }
    }
}
