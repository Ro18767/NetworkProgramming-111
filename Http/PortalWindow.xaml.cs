using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace Http
{
    /// <summary>
    /// Interaction logic for PortalWindow.xaml
    /// </summary>
    public partial class PortalWindow : Window
    {
        public PortalWindow()
        {
            InitializeComponent();
        }

        private void ChatServer_Click(object sender, RoutedEventArgs e)
        {
            String currentDir = Directory.GetCurrentDirectory();

            String solutionPath = currentDir.Substring(0, currentDir.IndexOf("Http"));
            String appPath = solutionPath + @"Server\bin\Debug\net6.0-windows\Server.exe";

            Process p = Process.Start(appPath);
            // TODO ограничить запуск сервера если он уже запушен то выдавать вопрос запаускать ли еще один сервер
        }

        private void ChatCLient_Click(object sender, RoutedEventArgs e)
        {
            String currentDir = Directory.GetCurrentDirectory();

            String solutionPath = currentDir.Substring(0, currentDir.IndexOf("Http"));
            String appPath = solutionPath + @"CLient\bin\Debug\net6.0-windows\CLient.exe";

            Process p = Process.Start(appPath);
        }

        private void httpRequests_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
        }

        private void apiRequests_Click(object sender, RoutedEventArgs e)
        {
            new ApiWindow().Show();
        }

        private void SMTP_Click(object sender, RoutedEventArgs e)
        {
            new SMTPWindow().Show();
        }
    }
}
