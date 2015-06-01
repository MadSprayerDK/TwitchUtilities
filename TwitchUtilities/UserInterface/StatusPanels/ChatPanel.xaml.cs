using System.Windows;
using Twitch.Api;
using Twitch.Api.HttpServer;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for ChatPanel.xaml
    /// </summary>
    public partial class ChatPanel
    {
        public ChatPanel()
        {
            InitializeComponent();
            LoginPanel.OAuthClient.HttpServer.OAuthCodeRecived += OauthCodeRecived;
        }

        private async void OauthCodeRecived(object sender, StringEventArg args)
        {
            LoginPanel.OAuthClient.HttpServer.Stop();
            var accessToken = await LoginPanel.OAuthClient.GetAccessToken(args.Text);

            LoginPanel.Dispatcher.Invoke(() => { LoginPanel.Visibility = Visibility.Hidden; });
            TextPanel.Dispatcher.Invoke(() => { TextPanel.Visibility = Visibility.Visible; });

            var channel = LoginPanel.Dispatcher.Invoke(() => LoginPanel.Channel.Text);
            TextPanel.Connect(channel, accessToken, channel);

            
        }
    }
}
