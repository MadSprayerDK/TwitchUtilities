using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TwitchUtilities.UserInterface.LabelsPanels
{
    /// <summary>
    /// Interaction logic for CustomLabelPanel.xaml
    /// </summary>
    public partial class CustomLabelPanel
    {
        private readonly Dictionary<string, string> _customLabelCache;

        public CustomLabelPanel()
        {
            InitializeComponent();

            _customLabelCache = new Dictionary<string, string>();
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
            var sendingVariable = (CustomLabelVariable)sender;
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
    }
}
