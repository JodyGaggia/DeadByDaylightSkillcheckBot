using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace NoMoreBoredom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow main;
        public MainWindow()
        {
            InitializeComponent();
            main = this;
            GlobalHotkey inputHandler = new GlobalHotkey();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var parent = sender as FrameworkElement;
            if (parent == null)
                return;
            topPanel.Width = parent.ActualWidth;
        }
    }
}
