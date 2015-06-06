using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
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

            _api = new TwitchApi(Settings.Default.CurrentChannel, Settings.Default.OAuthToken);
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

        private async void GameToggleEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (Game.Visibility == Visibility.Visible)
            {
                Game.Visibility = Visibility.Hidden;
                GameEdit.Text = Game.Content.ToString();
                GameEdit.IsEnabled = true;
                GameEdit.Visibility = Visibility.Visible;
                GameEditCancel.Visibility = Visibility.Visible;
            }
            else
            {
                GameEdit.IsEnabled = false;
                await _api.UpdateGame(GameEdit.Text);
                Game.Content = GameEdit.Text;
                Game.Visibility = Visibility.Visible;
                GameEdit.Visibility = Visibility.Hidden;
                GameEditCancel.Visibility = Visibility.Hidden;
            }
        }

        private void GameEditCancle_OnClick(object sender, RoutedEventArgs e)
        {
            GameEdit.IsEnabled = false;
            Game.Visibility = Visibility.Visible;
            GameEdit.Visibility = Visibility.Hidden;
            GameEditCancel.Visibility = Visibility.Hidden;
        }

        private async void StatusEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (Status.Visibility == Visibility.Visible)
            {
                Status.Visibility = Visibility.Hidden;
                StatusEdit.Text = Status.Content.ToString();
                StatusEdit.IsEnabled = true;
                StatusEdit.Visibility = Visibility.Visible;
                StatusEditCancel.Visibility = Visibility.Visible;;
            }
            else
            {
                StatusEdit.IsEnabled = false;
                await _api.UpdateStatus(StatusEdit.Text);
                Status.Content = StatusEdit.Text;
                StatusEdit.Visibility = Visibility.Hidden;
                Status.Visibility = Visibility.Visible;
                StatusEditCancel.Visibility = Visibility.Hidden;
            }
        }

        private void StatusEditCancle_OnClick(object sender, RoutedEventArgs e)
        {
            StatusEdit.IsEnabled = false;
            StatusEdit.Visibility = Visibility.Hidden;
            Status.Visibility = Visibility.Visible;
            StatusEditCancel.Visibility = Visibility.Hidden;
        }

        private async void GameEdit_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(GameEdit.Text) || NonCharButton(e.Key))
                return;

            try
            {
                var result = await _api.SearchGames(GameEdit.Text);
                GameEdit.ItemsSource = result.Games.Select(x => x.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool NonCharButton(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                case Key.Insert:
                case Key.Scroll:
                case Key.Print:
                case Key.PrintScreen:
                case Key.Pause:
                case Key.Home:
                case Key.End:
                case Key.NumLock:
                case Key.Tab:
                case Key.Delete:
                case Key.PageUp:
                case Key.PageDown:
                case Key.Enter:
                case Key.CapsLock:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LWin:
                case Key.RWin:
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                case Key.F1:
                case Key.F2:
                case Key.F3:
                case Key.F4:
                case Key.F5:
                case Key.F6:
                case Key.F7:
                case Key.F8:
                case Key.F9:
                case Key.F10:
                case Key.F11:
                case Key.F12:
                    return true;
                default:
                    return false;
            }
        }
    }
}
