using System.IO;
using System.Windows;

namespace TwitchUtilities.UserInterface
{
    /// <summary>
    /// Interaction logic for Checklist.xaml
    /// </summary>
    public partial class Checklist
    {
        public Checklist()
        {
            InitializeComponent();

            PreChecklist.Text = File.ReadAllText("Settings/PreChecklist.txt");
            PostChecklist.Text = File.ReadAllText("Settings/PostChecklist.txt");
        }

        private void SaveChecklists_OnClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Settings/PreChecklist.txt", PreChecklist.Text);
            File.WriteAllText("Settings/PostChecklist.txt", PostChecklist.Text);
        }
    }
}
