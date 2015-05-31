using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Twitch.Api;
using Twitch.Api.Model;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for StatusPanel.xaml
    /// </summary>
    public partial class StatusPanel
    {
        private readonly Timer _updateChannelTimer;

        public StatusPanel()
        {
            InitializeComponent();

            _updateChannelTimer = new Timer
            {
                Interval = 20000,
                AutoReset = true
            };
            _updateChannelTimer.Elapsed += ChannelDataTimer_Elapsed;
        }

        private async void GetChannelData_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ChannelName.Text))
                return;

            UpdateChannelDataButton.IsEnabled = false;

            await Task.Run(() =>
            {
                ChannelDataTimer_Elapsed(this, null);
            });

            _updateChannelTimer.Start();
        }

        private async void ChannelDataTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            var channelName = ChannelName.Dispatcher.Invoke(() => ChannelName.Text);
            var api = new TwitchApi(channelName);

            StreamInfo stream;
            try
            {
                stream = await api.GetStream();
            }
            catch (Exception e)
            {
                _updateChannelTimer.Stop();
                MessageBox.Show(e.Message);
                return;
            }
            finally
            {
                UpdateChannelDataButton.Dispatcher.Invoke(() => { UpdateChannelDataButton.IsEnabled = true; });
            }

            Channel channel;

            if (stream.Stream == null)
                channel = await api.GetChannel();
            else
                channel = stream.Stream.Channel;

            Status_CurrentViewers.Dispatcher.Invoke(() =>
            {
                Status_CurrentViewers.Content = stream.Stream == null ? "-" : stream.Stream.Viewers.ToString();
            });

            Status_Followers.Dispatcher.Invoke(() =>
            {
                Status_Followers.Content = channel.Followers.ToString();
            });

            Status_Game.Dispatcher.Invoke(() =>
            {
                Status_Game.Content = channel.Game;
            });

            Status_LiveStatus.Dispatcher.Invoke(() =>
            {
                Status_LiveStatus.Content = stream.Stream == null ? "Not Live" : "Live!";
            });

            Status_Views.Dispatcher.Invoke(() =>
            {
                Status_Views.Content = channel.Views.ToString();
            });

            Status_Status.Dispatcher.Invoke(() =>
            {
                Status_Status.Content = channel.Status;
            });
        }

        private void ChannelName_TextChange(object sender, TextChangedEventArgs e)
        {
            _updateChannelTimer.Stop();
        }

        private void ChannelName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            e.Handled = true;
            GetChannelData_OnClick(this, null);
        }
    }
}
