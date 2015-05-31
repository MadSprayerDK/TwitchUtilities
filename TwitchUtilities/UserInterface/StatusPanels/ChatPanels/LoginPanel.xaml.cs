using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using Twitch.Api;
using Twitch.Api.HttpServer;

namespace TwitchUtilities.UserInterface.StatusPanels.ChatPanels
{
    /// <summary>
    /// Interaction logic for LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel
    {
        public readonly OAuth OAuthClient;

        public LoginPanel()
        {
            InitializeComponent();
            OAuthClient = new OAuth();
        }

        private void BeginLogin_OnClick(object sender, RoutedEventArgs e)
        {
            OAuthClient.GetAuthLink();
            OAuthClient.GotoAuthorization();
            LoginButton.IsEnabled = false;
        }
    }
}
