using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace TwitchUtilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private TimeSpan _counting;
        private readonly Timer _countdownTimer;

        public MainWindow()
        {
            InitializeComponent();

            CountdownOutputLabel.Content = "00:00:00";

            _countdownTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };

            _countdownTimer.Elapsed += (o, args) =>
            {
                if (_counting.TotalSeconds < 1)
                    _countdownTimer.Stop();
                else
                {
                    _counting = _counting - new TimeSpan(0, 0, 0, 1);

                    var output = _counting.ToString(@"hh\:mm\:ss");
                    CountdownOutputLabel.Dispatcher.Invoke(() => { CountdownOutputLabel.Content = output; });

                    var countdownText = CountdownTextInput.Dispatcher.Invoke(() => CountdownTextInput.Text);

                    if (string.IsNullOrEmpty(countdownText))
                        File.WriteAllText("Labels/Countdown.txt", output);
                    else
                        File.WriteAllText("Labels/Countdown.txt", countdownText + @" " + output);
                }
            };

            PreChecklist.Text = File.ReadAllText("Settings/PreChecklist.txt");
            PostChecklist.Text = File.ReadAllText("Settings/PostChecklist.txt");
        }

        private void UpdaetLabelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Labels/CustomLabel.txt", CustomLabel.Text);
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

                var output = _counting.ToString(@"hh\:mm\:ss");
                if (string.IsNullOrEmpty(CountdownTextInput.Text))
                    File.WriteAllText("Labels/Countdown.txt", output);
                else
                    File.WriteAllText("Labels/Countdown.txt", CountdownTextInput.Text + @" " + output);
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

            var output = _counting.ToString(@"hh\:mm\:ss");
            if (string.IsNullOrEmpty(CountdownTextInput.Text))
                File.WriteAllText("Labels/Countdown.txt", output);
            else
                File.WriteAllText("Labels/Countdown.txt", CountdownTextInput.Text + @" " + output);
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
            var output = _counting.ToString(@"hh\:mm\:ss");
            CountdownOutputLabel.Content = output;
        }

        private void ClearCountdown_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Labels/Countdown.txt", "");
        }

        private void SaveChecklists_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Settings/PreChecklist.txt", PreChecklist.Text);
            File.WriteAllText("Settings/PostChecklist.txt", PostChecklist.Text);
        }
    }
}
