using System.Windows;

namespace ScreenRecorder
{
    public partial class Identify : Window
    {
        public Identify()
        {
            InitializeComponent();
            Top = 0;
            Left = 0;
            ScreenIdentifierNum.Content = 1;
        }
    }
}