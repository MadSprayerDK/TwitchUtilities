using System.Windows;
using ChatSharp;

namespace TwitchUtilities.UserInterface.StatusPanels.ChatPanels
{
    /// <summary>
    /// Interaction logic for TextPanel.xaml
    /// </summary>
    public partial class TextPanel
    {
        private IrcClient ircClient;
        public TextPanel()
        {
            InitializeComponent();
        }

        public void Connect(string username, string oAuthKey)
        {
            ircClient = new IrcClient("irc.twitch.tv", new IrcUser(username, "oauth:" + oAuthKey));
            ircClient.NetworkError += (s, e) => MessageBox.Show("Error: " + e.SocketError);
            ircClient.ConnectionComplete += (s, e) =>
            {
                ircClient.JoinChannel("#" + username);
            };

            ircClient.ChannelMessageRecieved += (s, e) =>
            {
                ChatOutput.Dispatcher.Invoke(() => { ChatOutput.Text += "\n" + e.PrivateMessage.User.Nick + ":" +  e.PrivateMessage.Message; });
            };

            ircClient.ConnectAsync();
        }
    }
}
