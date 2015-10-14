using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Api.TwitchAlerts;
using mmOAuth.Core.HttpServer;
using TwitchUtilities.Properties;
using Timer = System.Timers.Timer;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for DonationsPanel.xaml
    /// </summary>
    public partial class DonationsPanel
    {
        private static readonly string[] Scopes =
        {
            "donations.read",
            "donations.create"
        };

        private mmOAuth.Core.OAuth _oAuth;
        private TwitchAlertsApi _api;
        private Timer _updateDonations;
        private bool _onLoadCalled;

        public DonationsPanel()
        {
            InitializeComponent();
        }

        private async void DonationsPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_onLoadCalled)
                return;

            _onLoadCalled = true;

            if (string.IsNullOrEmpty(Settings.Default.TwitchAlertsOAuthToken))
            {
                _oAuth = new mmOAuth.Core.OAuth(new mmOAuth.TwitchAlerts.Provider(
                                Scopes,
                                OAuthInfo.TwitchAlertsOauthInfo.ClientId,
                                OAuthInfo.TwitchAlertsOauthInfo.Secret,
                                "http://localhost:8080",
                                "WebPages/success_twitch.html"));

                _oAuth.OAuthCodeRecived += OAuth_OnOAuthCodeRecived;

                DonationLoginLabel.Content = "No token found. Please log in.";
                DonationsLoginButton.IsEnabled = true;
            }
            else
            {
                _api = new TwitchAlertsApi(Settings.Default.TwitchAlertsOAuthToken);

                _updateDonations = new Timer
                {
                    Interval = 20000,
                    AutoReset = true
                };

                _updateDonations.Elapsed += _updateDonations_Elapsed;

                DonationGrid.RowDefinitions[0].Height = new GridLength(0);

                DonationsLogin.Visibility = Visibility.Collapsed;
                DonationsDisplay.Visibility = Visibility.Visible;

                if (Settings.Default.LastDonationId == 0)
                {
                    Settings.Default.LastDonationId = await _api.GetLastDonationId();
                    Settings.Default.Save();
                }

                await Task.Run(() =>
                {
                    _updateDonations_Elapsed(this, null);
                });

                _updateDonations.Start();
            }
        }

        private async void OAuth_OnOAuthCodeRecived(object sender, StringEventArg args)
        {
            var accessToken = await _oAuth.GetAccessToken(args.Text);
            Settings.Default.TwitchAlertsOAuthToken = accessToken;
            Settings.Default.Save();

            Dispatcher.Invoke(() =>
            {
                _onLoadCalled = false;
                DonationsPanel_OnLoaded(this, null);
            });
        }

        private void DonationsLogin_OnClick(object sender, RoutedEventArgs e)
        {
            _oAuth.StartOAuthRedirectServer();
            _oAuth.GotoAuthorization();

            DonationLoginLabel.Content = "Authorizing...";
            DonationsLoginButton.IsEnabled = false;
        }

        private async void _updateDonations_Elapsed(object sender, ElapsedEventArgs e)
        {
            var donations = await _api.GetLastDonations(Settings.Default.LastDonationId);

            foreach (var donation in donations.OrderBy(x => x.Created_At))
            {
                var donation1 = donation;
                Dispatcher.Invoke(() =>
                {
                    DonationsDisplayContent.Children.Add(new DonationDisplay(donation1.Name, donation1.Message, donation1.Created_At, donation1.Currency, donation1.Amount));
                    DonationsDisplay.ScrollToBottom();

                    Settings.Default.LastDonationId = donation1.Donation_Id;
                    Settings.Default.Save();
                });
            }
        }
    }
}
