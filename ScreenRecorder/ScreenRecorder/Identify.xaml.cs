﻿using System.Windows;

namespace ScreenRecorder
{
    public partial class Identify : Window
    {
        public Identify(int screenNum, int x, int y)
        {
            InitializeComponent();
            Title = "Screen Identifier";
            Top = y;
            Left = x;
            ScreenIdentifierNum.Content = screenNum.ToString();
        }
    }
}