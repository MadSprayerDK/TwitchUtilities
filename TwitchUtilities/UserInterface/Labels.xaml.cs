using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using TwitchUtilities.UserInterface.LabelsPanels;

namespace TwitchUtilities.UserInterface
{
    /// <summary>
    /// Interaction logic for Labels.xaml
    /// </summary>
    public partial class Labels
    {
        private TimeSpan _counting;
        private readonly Timer _countdownTimer;

        private readonly Dictionary<string, string> _customLabelCache;

        public Labels()
        {
            InitializeComponent();

            CountdownTextInput.Text = "You can use the {time} identifier here";
            _customLabelCache = new Dictionary<string, string>();

            _countdownTimer = new Timer
            {
                Interval = 1000,
                AutoReset = true,
            };
            _countdownTimer.Elapsed += CountdownTimer_Elapsed;
        }

        private void UpdateLabelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Labels/CustomLabel.txt", GetCustomLabelContent());
        }

        private void CustomLabel_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var activeKeys = new List<string>();
            var matches = Regex.Matches(CustomLabel.Text, @"\{(\s*?.*?)\}");

            foreach (Match match in matches)
            {
                var key = match.Value.Substring(1, match.Value.Length - 2);

                if (!_customLabelCache.ContainsKey(key))
                    _customLabelCache.Add(key, "");

                activeKeys.Add(key);
            }

            CustomVariables.Children.Clear();

            foreach (var key in activeKeys)
            {
                var newVariable = new CustomLabelVariable(key, _customLabelCache[key]);
                newVariable.InputChanged += CustomLabelVariableChanged;
                CustomVariables.Children.Add(newVariable);
            }

            if (activeKeys.Any())
            {
                var text = GetCustomLabelContent();
                CustomLabelPreview.Content = text.Length == 0 ? "<Label output preview>" : text;
                CustomLabelPreview.Visibility = Visibility.Visible;
            }
            else
                CustomLabelPreview.Visibility = Visibility.Collapsed;
        }

        private void CustomLabelVariableChanged(object sender, EventArgs args)
        {
            var sendingVariable = (CustomLabelVariable) sender;
            _customLabelCache[sendingVariable.Title] = sendingVariable.Value;

            CustomLabelPreview.Content = GetCustomLabelContent();
        }

        private string GetCustomLabelContent()
        {
            var resultString = CustomLabel.Text;

            foreach (var variable in _customLabelCache)
            {
                if (resultString.IndexOf("{" + variable.Key + "}", StringComparison.Ordinal) != -1)
                    resultString = resultString.Replace("{" + variable.Key + "}", variable.Value);
            }

            return resultString;
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
