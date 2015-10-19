using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwitchUtilities.UserInterface.VotePanels
{
    /// <summary>
    /// Interaction logic for VoteOption.xaml
    /// </summary>
    public partial class VoteOption : UserControl
    {
        public delegate void OptionsRemovedClicked(object sender, EventArgs args);
        public event OptionsRemovedClicked RemoveClicked;

        public string Name
        {
            set { OptionName.Text = value; }
            get { return OptionName.Text; }
        }

        public string Value
        {
            set { OptionValue.Text = value; }
            get { return OptionValue.Text; }
        }

        public int Percentage
        {
            set { OptionPercent.Content = value + "%"; }
            get
            {
                var content = OptionPercent.Content.ToString();
                return int.Parse(content.Remove(content.Length - 1));
            }
        }

        public VoteOption()
        {
            InitializeComponent();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveClicked != null)
                RemoveClicked(this, null);
        }
    }
}
