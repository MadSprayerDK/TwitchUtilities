using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatSharp;
using ChatSharp.Events;
using TwitchUtilities.Properties;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for ChatPanel.xaml
    /// </summary>
    public partial class ChatPanel
    {
        private IrcClient _irc;
        private string _channel;

        public ChatPanel()
        {
            InitializeComponent();

            var channel = Settings.Default.CurrentChannel;
            var oAuthToken = Settings.Default.OAuthToken;
            Connect(channel, oAuthToken, channel);
        }

        private void PrintToOutput(string message, bool newline = true)
        {
            ChatOutput.Dispatcher.Invoke(() => { ChatOutput.Text += (newline ? "\n" : "") + message; });
        }

        private void Connect(string username, string oAuthKey, string channel)
        {
            _channel = channel;
            _irc = new IrcClient("irc.twitch.tv", new IrcUser(username, username, "oauth:" + oAuthKey));

            _irc.ConnectionComplete += Irc_OnConnectionComplete;
            _irc.NetworkError += Irc_OnNetworkError;
            _irc.ChannelMessageRecieved += Irc_OnChannelMessageRecieved;
            _irc.UserJoinedChannel += Irc_OnUserJoinedChannel;
            _irc.UserPartedChannel += Irc_OnUserPartedChannel;
            _irc.UserKicked += Irc_OnUserKicked;

            PrintToOutput(">> Connecting...");
            _irc.ConnectAsync();
        }

        private void Irc_OnUserKicked(object sender, KickEventArgs kickEventArgs)
        {
            PrintToOutput(">> User was kicked: " + kickEventArgs.Kicked.User + ", By:" + kickEventArgs.Kicker.User +
                          ". Reason: " + kickEventArgs.Reason);
        }

        private void Irc_OnUserPartedChannel(object sender, ChannelUserEventArgs channelUserEventArgs)
        {
            PrintToOutput(">> User left the channel: " + channelUserEventArgs.User.User);
        }

        private void Irc_OnUserJoinedChannel(object sender, ChannelUserEventArgs channelUserEventArgs)
        {
            PrintToOutput(">> User joined the channel: " + channelUserEventArgs.User.User);
        }

        private void Irc_OnChannelMessageRecieved(object sender, PrivateMessageEventArgs privateMessageEventArgs)
        {
            PrintToOutput(privateMessageEventArgs.PrivateMessage.User.Nick + ": " + privateMessageEventArgs.PrivateMessage.Message);
        }

        private void Irc_OnNetworkError(object sender, SocketErrorEventArgs socketErrorEventArgs)
        {
            PrintToOutput(">> Error: " + socketErrorEventArgs.SocketError);
        }

        private void Irc_OnConnectionComplete(object sender, EventArgs eventArgs)
        {
            PrintToOutput(">> Connected.");
            PrintToOutput(">> Joining channel: #" + _channel);
            _irc.JoinChannel("#" + _channel);
        }

        private void SendMessage_Onclick(object sender, RoutedEventArgs e)
        {
            var channel = _irc.Channels["#" + _channel];
            channel.SendMessage(ChatInput.Text);
            PrintToOutput(_irc.User.User + ": " + ChatInput.Text);
            ChatInput.Text = "";
        }

        private void ChatInput_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage_Onclick(this, null);
        }

        private void ChatOutput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ChatOutput.ScrollToEnd();
        }
    }
}
