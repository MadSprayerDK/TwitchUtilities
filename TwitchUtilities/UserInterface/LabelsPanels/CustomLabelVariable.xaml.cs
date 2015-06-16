using System;
using System.Windows.Controls;

namespace TwitchUtilities.UserInterface.LabelsPanels
{
    /// <summary>
    /// Interaction logic for CustomLabelVariable.xaml
    /// </summary>
    public partial class CustomLabelVariable
    {
        public delegate void InputChangedEventHandler(object sender, EventArgs e);
        public event InputChangedEventHandler InputChanged;

        public string Title
        {
            set { VariableTitle.Content = value; }
            get { return VariableTitle.Content.ToString(); }
        }

        public string Value
        {
            set { VariableText.Text = value; } 
            get { return VariableText.Text; }
        }

        public CustomLabelVariable()
        {
            InitializeComponent();
        }

        public CustomLabelVariable(string title, string value)
        {
            InitializeComponent();
            Title = title;
            Value = value;
        }

        private void VariableText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (InputChanged != null)
                InputChanged(this, e);
        }
    }
}
