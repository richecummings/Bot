using System.Threading;
using Bot.Configuration;
using Bot.ViewModel;
using Xamarin.Forms;

namespace Bot.View
{
    public partial class MainPage : ContentPage
    {
        MainVM viewModel;
        public MainPage()
        {
            InitializeComponent();

            viewModel = Resources["vm"] as MainVM;

            viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var newMessage = viewModel.Messages[viewModel.Messages.Count - 1];
            Device.BeginInvokeOnMainThread(() =>
            {
                chatListView.ScrollTo(newMessage, ScrollToPosition.End, true);
            });
        }
    }
}
