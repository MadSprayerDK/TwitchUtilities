using System;
using System.IO;
using System.Timers;
using System.Windows;
using Api.Spotify;

namespace TwitchUtilities.UserInterface.LabelsPanels
{
    /// <summary>
    /// Interaction logic for SpotifyLabelPanel.xaml
    /// </summary>
    public partial class SpotifyLabelPanel
    {
        private readonly SpotifyApi _spotify;
        private Timer _refreshTimer;

        public SpotifyLabelPanel()
        {
            InitializeComponent();
            _spotify = new SpotifyApi("tu.spotilocal.com");
        }

        private async void SpotifyLabelPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            await _spotify.Init();

            _refreshTimer = new Timer
            {
                Interval = 2500,
                AutoReset = true,
            };
            _refreshTimer.Elapsed += _refreshTimer_Elapsed;
            _refreshTimer.Start();
        }

        private async void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var status = await _spotify.GetStatus();
            var output = status.Playing ? status.Track.Artist_Resource.Name + " - " + status.Track.Track_Resource.Name : "No song playing.";

            Dispatcher.Invoke(() =>
            {
                SpotifyOutput.Content = output;
            });
            try
            {
                File.WriteAllText("Labels/SpotifyInfo.txt", output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception:" + ex.Message);
            }
            
        }
    }
}
