using System.Linq;
using System.Windows;
using Twitch.Api;
using Twitch.Api.HttpServer;
using TwitchUtilities.Properties;

namespace TwitchUtilities
{
    /// <summary>
    /// Interaction logic for OAuthLogin.xaml
    /// </summary>
    public partial class OAuthLogin
    {
        private readonly OAuth _oAuth;
        public OAuthLogin()
        {
            InitializeComponent();
            _oAuth = new OAuth();
            _oAuth.HttpServer.OAuthCodeRecived += HttpServer_OnOAuthCodeRecived;
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

            if (!validatedToken.Token.Valid)
            {
                StatusText.Content = "Token is invalid. Please login.";
                LoginButton.IsEnabled = true;
                return;
            }

            if (!OAuth.Scopes.All(scope => validatedToken.Token.Authorization.Scopes.Contains(scope)))
            {
                StatusText.Content = "Token is missing permissions. Please login.";
                LoginButton.IsEnabled = true;
                return;
            }

            Settings.Default.CurrentChannel = validatedToken.Token.User_Name;
            Settings.Default.Save();

            var window = new MainWindow();
            window.Show();
            Close();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusText.Content = "Authorizing...";
            LoginButton.IsEnabled = false;
            _oAuth.StartAuthRedirectServer();
            _oAuth.GotoAuthorization();
        }

        private async void HttpServer_OnOAuthCodeRecived(object sender, StringEventArg args)
        {
            var accessCode = await _oAuth.GetAccessToken(args.Text);
            Settings.Default.OAuthToken = accessCode;
            Settings.Default.Save();

            Dispatcher.Invoke(() => { OAuthLogin_OnLoaded(this, null); });
        }
    }
}
