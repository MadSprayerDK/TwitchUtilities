using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Twitch.Api;
using Twitch.Api.Model;
using TwitchUtilities.Properties;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for StatusPanel.xaml
    /// </summary>
    public partial class StatusPanel
    {
        private readonly Timer _updateChannelTimer;
        private readonly TwitchApi _api;

        public StatusPanel()
        {
            InitializeComponent();

            _updateChannelTimer = new Timer
            {
                Interval = 20000,
                AutoReset = true
            };
            _updateChannelTimer.Elapsed += ChannelDataTimer_Elapsed;

            _api = new TwitchApi(Settings.Default.CurrentChannel);
        }

        private async void StatusPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                ChannelDataTimer_Elapsed(this, null);
            });

            _updateChannelTimer.Start();
        }

        private async void ChannelDataTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            StreamInfo stream;
            try
            {
                stream = await _api.GetStream();
            }
            catch (Exception e)
            {
                _updateChannelTimer.Stop();
                MessageBox.Show(e.Message);
                return;
            }

            Channel channel;

            if (stream.Stream == null)
                channel = await _api.GetChannel();
            else
                channel = stream.Stream.Channel;

            Dispatcher.Invoke(() =>
            {
                CurrentViewers.Content = stream.Stream == null ? "-" : stream.Stream.Viewers.ToString();
                Followers.Content = channel.Followers.ToString();
                Game.Content = channel.Game;
                LiveStatus.Content = stream.Stream == null ? "Not Live" : "Live!";
                Views.Content = channel.Views.ToString();
                Status.Content = channel.Status;
            });
        }

        
    }
}
