﻿using System.Linq;
using System.Windows;
using ManoSoftware.mmOAuth.Core.HttpServer;
using TwitchUtilities.Properties;
namespace TwitchUtilities
{
    /// <summary>
    /// Interaction logic for OAuthLogin.xaml
    /// </summary>
    public partial class OAuthLogin
    {
        private static readonly string[] Scopes =
        {
            "chat_login", 
            "channel_editor"
        };

        private readonly ManoSoftware.mmOAuth.Core.OAuth _oAuth;
        public OAuthLogin()
        {
            InitializeComponent();
            _oAuth = new ManoSoftware.mmOAuth.Core.OAuth(
                new ManoSoftware.mmOAuth.Twitch.Provider(
                    Scopes, 
                    OAuthInfo.TwitchOauthInfo.ClientId,
                    OAuthInfo.TwitchOauthInfo.Secret,
                    "http://localhost:8080",
                    "WebPages/success_twitch.html"));
            _oAuth.OAuthCodeRecived += OAuth_OnOAuthCodeRecived;
        }

        private async void OAuthLogin_OnLoaded(object sender, RoutedEventArgs e)
        {
            var token = Settings.Default.OAuthToken;

            if (string.IsNullOrEmpty(token))
            {
                StatusText.Content = "No token found. Please login.";
                LoginButton.IsEnabled = true;
                return;
            }

            var validatedToken = await _oAuth.VerifyToken(token);

            if (!validatedToken.Valid)
            {
                StatusText.Content = "Token is invalid. Please login.";
                LoginButton.IsEnabled = true;
                return;
            }

            if (!Scopes.All(scope => validatedToken.Scope.Contains(scope)))
            {
                StatusText.Content = "Token is missing permissions. Please login.";
                LoginButton.IsEnabled = true;
                return;
            }

            Settings.Default.CurrentChannel = validatedToken.UserName;
            Settings.Default.Save();

            var window = new MainWindow();
            window.Show();
            Close();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusText.Content = "Authorizing...";
            LoginButton.IsEnabled = false;
            _oAuth.StartOAuthRedirectServer();
            _oAuth.GotoAuthorization();
        }

        private async void OAuth_OnOAuthCodeRecived(object sender, StringEventArg args)
        {
            var accessCode = await _oAuth.GetAccessToken(args.Text);
            Settings.Default.OAuthToken = accessCode;
            Settings.Default.Save();

            Dispatcher.Invoke(() =>
            {
                StatusText.Content = "Verifying Token...";
                OAuthLogin_OnLoaded(this, null);
            });
        }
    }
}
