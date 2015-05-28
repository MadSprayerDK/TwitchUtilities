using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwitchUtilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdaetLabelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Labels/CustomLabel.txt", CustomLabel.Text);
        }

        private void StartCountdown_OnClick(object sender, RoutedEventArgs e)
        {
            double hours;
            double minutes;
            double seconds;

            double.TryParse(CountdownHours.Text, out hours);
            double.TryParse(CountdownMinutes.Text, out minutes);
            double.TryParse(CountdownSeconds.Text, out seconds);


            var targetDateTime = DateTime.Now.
                AddHours(hours)
                .AddMinutes(minutes)
                .AddSeconds(seconds);

            var timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += (o, args) =>
            {
                var difference = targetDateTime - DateTime.Now;

                if(Math.Abs(difference.TotalSeconds) < 1)
                    timer.Stop();
                
                var output = difference.ToString(@"hh\:mm\:ss");
                CountdownOutputLabel.Dispatcher.BeginInvoke((Action)(() => CountdownOutputLabel.Content = output));
            };
            timer.AutoReset = true;
            timer.Start();

            //throw new NotImplementedException();
        }
    }
}
