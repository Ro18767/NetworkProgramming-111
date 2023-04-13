using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

namespace Http
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();
            httpClient = new();
        }
        private async void printResponse(HttpResponseMessage response)
        {

            resultTextBlock.Text = $"HTTP/{response.Version} {(int)response.StatusCode} {response.StatusCode} {response.ReasonPhrase}\n";
            foreach (var header in response.Headers)
            {
                // header.Key;
                string headerSting = $"{header.Key}:\n";
                foreach (var val in header.Value)
                {
                    headerSting += $"\t{val}\n";
                }
                resultTextBlock.Text += headerSting;
            }
            resultTextBlock.Text += "\n ------------------------------------------ \n";
            resultTextBlock.Text += await response.Content.ReadAsStringAsync();
            resultTextBlock.Text += "\n ------------------------------------------ \n";
        }
        private async void get1Button_Click(object sender, RoutedEventArgs e)
        {
            String result =  await httpClient.GetStringAsync(url1TextBox.Text);
            resultTextBlock.Text = result;
        }

        private async void get2Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url2TextBox.Text);
            var response = await httpClient.SendAsync(request);
            printResponse(response);
        }

        private async void head3Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url3TextBox.Text);
            var response = await httpClient.SendAsync(request);
            printResponse(response);
        }

        private async void options4Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new HttpRequestMessage(HttpMethod.Options, url4TextBox.Text);
            var response = await httpClient.SendAsync(request);
            printResponse(response);
        }
        private async void get5Button_Click(object sender, RoutedEventArgs e)
        {
            String result = await httpClient.GetStringAsync(url5TextBox.Text);
            var startSearch = "<ul class=\"data\">";
            var start = result.IndexOf(startSearch) + startSearch.Length;
            var fromStartStr = result.Substring(start);
            var endSearch = "</ul>";
            var end = fromStartStr.IndexOf(endSearch);
            var fromStartToEndStr = fromStartStr.Substring(0, end);
            var open = "<li>";
            var close = "</li>";
            resultTextBlock.Text = "";
            for (int i = 0; i < 5; i++)
            {
                if (fromStartStr.IndexOf(open) == -1) break;
                var pass = fromStartToEndStr.Substring(fromStartStr.IndexOf(open) + open.Length, fromStartStr.IndexOf(close) - fromStartStr.IndexOf(open) - open.Length);
                fromStartToEndStr = fromStartToEndStr.Substring(fromStartStr.IndexOf(close) + close.Length);
                resultTextBlock.Text += $"{pass}\n";
            }
            
        }

    }
}
// https://api.monobank.ua/docs/#tag/Publichni-dani
