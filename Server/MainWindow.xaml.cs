using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket? listenSocket;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint endpoint;
            try
            {
                IPAddress ip = IPAddress.Parse(serverIp.Text);
                int port = Convert.ToInt32(serverPort.Text);
                endpoint = new(ip, port);
            }
            catch
            {
                MessageBox.Show("Check network params");
                return;
            }
            listenSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            new Thread(StartServerMethod).Start(endpoint);
        }

        private void StartServerMethod(object? o)
        {
            IPEndPoint? endpoint = o as IPEndPoint;
            if (endpoint is null) return;
            if (listenSocket is null) return;

            try
            {

                listenSocket.Bind(endpoint);
                listenSocket.Listen(100);
                byte[] buf = new byte[1024];
                Dispatcher.Invoke(() =>
                {
                    StartServer.IsEnabled = false;
                    StopServer.IsEnabled = true;
                    serverLogs.Text += "Server Started\n";
                    serverSatus.Content = "ON";
                    serverSatus.Foreground = new SolidColorBrush(new Color { A = 0xFF, G = 0xFF});
                });
                while (true)
                {
                    Socket socket = listenSocket.Accept();
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        int n = socket.Receive(buf);
                        sb.Append(Encoding.UTF8.GetString(buf, 0, n));
                    } while (socket.Available > 0);


                    var str = sb.ToString();
                    Dispatcher.Invoke(() => serverLogs.Text += str + "\n");

                    str = "Recived at " + DateTime.Now;
                    socket.Send(Encoding.UTF8.GetBytes(str));

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Dispose();
                }

            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    StartServer.IsEnabled = true;
                    StopServer.IsEnabled = false;
                    serverLogs.Text += "Server Stoped" + ex.Message + "\n";
                    serverSatus.Content = "OFF";
                    serverSatus.Foreground = new SolidColorBrush(new Color { A = 0xFF, R = 0xFF });
                });
            }
        }
        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            listenSocket?.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listenSocket?.Close();
        }
    }
}
