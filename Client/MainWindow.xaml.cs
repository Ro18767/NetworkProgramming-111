using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Socket? clientSocket;
        private void SendButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Check server network params");
                return;
            }
            clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                clientSocket.Connect(endpoint);
                {// send
                    clientSocket.Send(Encoding.UTF8.GetBytes(messageTextBox.Text));
                }
                {// recive
                    byte[] buf = new byte[1024];
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        int n = clientSocket.Receive(buf);
                        sb.Append(Encoding.UTF8.GetString(buf, 0, n));
                    } while (clientSocket.Available > 0);

                    var str = sb.ToString();
                    chatLogs.Text += str + "\n";
                }
                {// close
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Dispose();
                }
            }
            catch (Exception ex)
            {
                chatLogs.Text += "Server Stoped" + ex.Message + "\n";
            }
        }
    }
}
