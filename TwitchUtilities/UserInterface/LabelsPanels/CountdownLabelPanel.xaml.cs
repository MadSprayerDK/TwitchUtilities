using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace TwitchUtilities.UserInterface.LabelsPanels
{
    /// <summary>
    /// Interaction logic for CountdownLabelPanel.xaml
    /// </summary>
    public partial class CountdownLabelPanel
    {
        private TimeSpan _counting;
        private readonly Timer _countdownTimer;

        public CountdownLabelPanel()
        {
            InitializeComponent();

            CountdownTextInput.Text = "You can use the {time} identifier here";

            _countdownTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true,
            };
            _countdownTimer.Elapsed += CountdownTimer_Elapsed;
        }

        private void ToggleCountdown_OnClick(object sender, RoutedEventArgs e)
        {
            if (_countdownTimer.Enabled)
                _countdownTimer.Stop();
            else
            {
                CountdownTextInput.IsEnabled = false;
                CountdownHours.IsEnabled = false;
                CountdownMinutes.IsEnabled = false;
                CountdownSeconds.IsEnabled = false;
                _countdownTimer.Start();

                CreateStreamString(true);
            }
        }

        private void ResetCountdown_OnClick(object sender, RoutedEventArgs e)
        {
            _countdownTimer.Stop();

            CountdownTextInput.IsEnabled = true;
            CountdownHours.IsEnabled = true;
            CountdownMinutes.IsEnabled = true;
            CountdownSeconds.IsEnabled = true;

            CountdownInsert_TextChange(this, null);
            CreateStreamString(true);
        }

        private void CountdownInsert_TextChange(object sender, TextChangedEventArgs e)
        {
            int hours;
            int minutes;
            int seconds;

            int.TryParse(CountdownHours.Text, out hours);
            int.TryParse(CountdownMinutes.Text, out minutes);
            int.TryParse(CountdownSeconds.Text, out seconds);

            _counting = new TimeSpan(0, hours, minutes, seconds);
            CreateStreamString(false);
        }

        private void CountdownTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            if (_counting.TotalSeconds < 1)
                _countdownTimer.Stop();
            else
            {
                _counting = _counting - new TimeSpan(0, 0, 0, 1);

                Dispatcher.Invoke(() =>
                {
                    CreateStreamString(true);
                });
            }
        }

        private void CreateStreamString(bool publish)
        {
            var counting = _counting.ToString(@"hh\:mm\:ss");
            var countdownTextInput = CountdownTextInput.Text;
            string output;

            if (string.IsNullOrEmpty(countdownTextInput))
                output = counting;
            else
            {
                if (countdownTextInput.IndexOf("{time}", StringComparison.Ordinal) != -1)
                    output = countdownTextInput.Replace("{time}", counting);
                else
                    output = CountdownTextInput.Text + @" " + counting;
            }

            CountdownOutputLabel.Content = output;

            if (publish)
                File.WriteAllText("Labels/Countdown.txt", output);
        }

        private void ClearCountdown_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Labels/Countdown.txt", "");
        }
    }
}
