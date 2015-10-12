using System.Windows;
using System.Windows.Controls;
using mmOAuth.Core.HttpServer;
using TwitchUtilities.Properties;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for DonationsPanel.xaml
    /// </summary>
    public partial class DonationsPanel : UserControl
    {
        private static readonly string[] Scopes =
        {
            "donations.read",
            "donations.create"
        };

        private readonly mmOAuth.Core.OAuth _oAuth;

        public DonationsPanel()
        {
            InitializeComponent();

            _oAuth = new mmOAuth.Core.OAuth(new mmOAuth.TwitchAlerts.Provider(
                Scopes,
                OAuthInfo.TwitchAlertsOauthInfo.ClientId,
                OAuthInfo.TwitchAlertsOauthInfo.Secret,
                "http://localhost:8080",
                "WebPages/success_twitch.html"));

            _oAuth.OAuthCodeRecived += OAuth_OnOAuthCodeRecived;
        }

        private void DonationsPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.TwitchAlertsOAuthToken))
            {
                DonationLoginLabel.Content = "No token found. Please log in.";
                DonationsLoginButton.IsEnabled = true;
            }
            else
            {
                DonationsLogin.Visibility = Visibility.Collapsed;
                DonationsDisplay.Visibility = Visibility.Visible;

                if (Settings.Default.LastDonationId == 0)
                {
                    
                }
            }
        }

        private async void OAuth_OnOAuthCodeRecived(object sender, StringEventArg args)
        {
            var accessToken = await _oAuth.GetAccessToken(args.Text);
            Settings.Default.TwitchAlertsOAuthToken = accessToken;
            Settings.Default.Save();

            Dispatcher.Invoke(() =>
            {
                DonationsLogin.Visibility = Visibility.Collapsed;
                DonationsDisplay.Visibility = Visibility.Visible;
            });
        }

        private void DonationsLogin_OnClick(object sender, RoutedEventArgs e)
        {
            _oAuth.StartOAuthRedirectServer();
            _oAuth.GotoAuthorization();

            DonationLoginLabel.Content = "Authorizing...";
            DonationsLoginButton.IsEnabled = false;
        }


        /*private int GetLastDonationId()
        {
            var client = new HttpClient { BaseAddress = new Uri(uri) };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(_provider.GetVerificationParameters(oAuthToken));
            var message = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return _provider.ExtractVerifiedMessage(message);
            }

            throw new Exception(_provider.ExtractErrorMessage(message));
        }*/
    }
}
