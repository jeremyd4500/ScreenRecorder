using System.Windows;

namespace ScreenRecorder
{
    public partial class Identify : Window
    {
        public Identify(int screenNum, int x)
        {
            InitializeComponent();
            Top = 0;
            Left = x;
            ScreenIdentifierNum.Content = screenNum;
        }
    }
}