using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ChatSharp;
using ChatSharp.Events;
using TwitchUtilities.Properties;
using TwitchUtilities.UserInterface.VotePanels;

namespace TwitchUtilities.UserInterface
{
    /// <summary>
    /// Interaction logic for Votes.xaml
    /// </summary>
    public partial class Votes
    {
        private readonly IrcClient _irc;
        private readonly Dictionary<string, string> _voteResults = new Dictionary<string, string>();
        private bool _activeVote;

        public Votes()
        {
            InitializeComponent();
            _irc = new IrcClient("irc.twitch.tv", new IrcUser(Settings.Default.CurrentChannel, Settings.Default.CurrentChannel, "oauth:" + Settings.Default.OAuthToken));
            _irc.ChannelMessageRecieved += Irc_ChannelMessageRecieved;
            _irc.ConnectionComplete += _irc_ConnectionComplete;
            _irc.ConnectAsync();

            ClearResultFromStream();
        }

        void _irc_ConnectionComplete(object sender, EventArgs e)
        {
            _irc.JoinChannel("#" + Settings.Default.CurrentChannel);
        }

        void Irc_ChannelMessageRecieved(object sender, PrivateMessageEventArgs e)
        {
            if (!_activeVote)
                return;

            Dispatcher.Invoke(() =>
            {
                var message = e.PrivateMessage.Message.ToLower();

                var options = OptionsContent.Children.Cast<VoteOption>().ToList();

                var votedOption = options.FirstOrDefault(x => message.Contains(x.Value.ToLower()));

                if (votedOption == null)
                    return;

                if (_voteResults.ContainsKey(e.PrivateMessage.User.Nick))
                    _voteResults[e.PrivateMessage.User.Nick] = votedOption.Value;
                else
                    _voteResults.Add(e.PrivateMessage.User.Nick, votedOption.Value);

                var total = _voteResults.Count;

                foreach (var option in options)
                {
                    var currentCount = _voteResults.Count(x => x.Value == option.Value);
                    var percent = (currentCount * 100) / total;
                    option.Percentage = percent;
                }

                if (ShowOnStream.IsChecked != null && ShowOnStream.IsChecked.Value)
                    ShowResultOnStream();
                else
                    ClearResultFromStream();
            });
        }

        private void AddOption_Click(object sender, RoutedEventArgs e)
        {
            var option = new VoteOption();
            option.RemoveClicked += Option_RemoveClicked;

            OptionsContent.Children.Add(option);
        }

        void Option_RemoveClicked(object sender, EventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                OptionsContent.Children.Remove((VoteOption)sender);

                ClearResultFromStream();

                if(OptionsContent.Children.Count == 0)
                    _voteResults.Clear();
            });
        }

        private void StartPause_Click(object sender, RoutedEventArgs e)
        {
            if (_activeVote)
            {
                StartPause.Content = "Start Vote";
                _activeVote = false;
                AddOption.IsEnabled = true;
                Reset.IsEnabled = true;

                foreach (VoteOption option in OptionsContent.Children)
                {
                    option.Remove.IsEnabled = true;
                    option.OptionName.IsEnabled = true;
                    option.OptionValue.IsEnabled = true;
                }
            }
            else
            {
                StartPause.Content = "Pause Vote";
                _activeVote = true;
                AddOption.IsEnabled = false;
                Reset.IsEnabled = false;

                if(ShowOnStream.IsChecked != null && ShowOnStream.IsChecked.Value)
                    ShowResultOnStream();

                foreach (VoteOption option in OptionsContent.Children)
                {
                    option.Remove.IsEnabled = false;
                    option.OptionName.IsEnabled = false;
                    option.OptionValue.IsEnabled = false;
                }
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            VoteName.Text = "";
            OptionsContent.Children.Clear();
            _voteResults.Clear();
            ClearResultFromStream();
        }

        private void ShowResultOnStream()
        {
            var outputString = VoteName.Text + "\n";

            foreach (VoteOption option in OptionsContent.Children)
            {
                outputString += option.Name + ": " + option.Value + " - " + option.Percentage + "%\n";
            }

            File.WriteAllText("Labels/VoteResult.txt", outputString);

        }

        private void ClearResultFromStream()
        {
            File.WriteAllText("Labels/VoteResult.txt", "");
        }
    }
}
