using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        byte[] buffer = new byte[1024];
        private Socket? clientSocket;
        IPEndPoint? endpoint;
        DateTime lastSyncMoment = DateTime.MinValue;

        void CreateEndPoint()
        {
            if (endpoint is not null) return;
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
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ChatMessage message = new()
            {
                Autor = autorTextBox.Text,
                Text = messageTextBox.Text,
                Moment = DateTime.Now,
            };
            SendMessage(message);
        }
        private void SendMessage(ChatMessage message)
        {
            ClientRequest request = new()
            {
                Action = "Message",
                Autor = message.Autor,
                Text = message.Text,
                Moment = message.Moment,

            };
            SendRequest(request);
        }
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            GetServerMessages();
        }
        private void GetServerMessages()
        {
            ClientRequest request = new()
            {
                Action = "Get",
                Autor = autorTextBox.Text,
                Text = String.Empty,
                Moment = lastSyncMoment,

            };
            ServerResponse? response = SendRequest(request);
            if (response is null) return;
            foreach(var message in response.Messages)
            {
                chatLogs.Text += $"{message.Moment.ToShortTimeString()} {message.Autor}: {message.Text}" + "\n";
            }
            lastSyncMoment = response.Messages.LastOrDefault()?.Moment ?? lastSyncMoment;
        }

        private ServerResponse? SendRequest(ClientRequest request)
        {
            CreateEndPoint();
            if (endpoint is null) return null;
            clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                clientSocket.Connect(endpoint);
                String json = JsonSerializer.Serialize(request, new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                {// send
                    clientSocket.Send(Encoding.UTF8.GetBytes(json));
                }
                {// recive
                    MemoryStream stream = new();
                    do
                    {
                        int n = clientSocket.Receive(buffer);
                        stream.Write(buffer, 0, n);
                    } while (clientSocket.Available > 0);

                    {// close
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Dispose();
                    }

                    var str = Encoding.UTF8.GetString(stream.ToArray());
                    ServerResponse? response = JsonSerializer.Deserialize<ServerResponse>(str);
                    if (response is null)
                    {
                        chatLogs.Text += $"Error: no response" + "\n";
                    } else
                    {
                        chatLogs.Text += $"Status:{response.Status}" + "\n";
                    }
                        
                    return response;
                }

            }
            catch (Exception ex)
            {
                chatLogs.Text += "Server Stoped" + ex.Message + "\n";
                return null;
            }
        }
    }
}
