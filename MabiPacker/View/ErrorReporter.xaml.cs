using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace MabiPacker.View
{
    /// <summary>
    /// ErrorReporter.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorReporter : MetroWindow
    {
        public ErrorReporter(string msg, string detail)
        {
            InitializeComponent();
            textBoxDetail.Text = detail;
            textBoxMessage.Text = msg;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(-1);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
