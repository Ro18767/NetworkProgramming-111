using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace Http
{
    /// <summary>
    /// Interaction logic for ApiWindow.xaml
    /// </summary>
    public partial class ApiWindow : Window
    {
        private HttpClient httpClient;
        private List<NBURate>? _nbuRates  { get; set; } = new();
        public ObservableCollection<NBURate> NBURates { get; set; } = new();
        public ApiWindow()
        {
            InitializeComponent();
            httpClient = new();
            this.DataContext = this;
        }

        private async void NBUToday_Click(object sender, RoutedEventArgs e)
        {
            String url = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
            // String json = await httpClient.GetStringAsync(url);
            // List<NBURate>? rates = JsonSerializer.Deserialize<List<NBURate>>(json);

            _nbuRates = await httpClient.GetFromJsonAsync<List<NBURate>>(url);
            if (_nbuRates is null)
            {
                MessageBox.Show("json err");
                return;
            };
            //StringBuilder sb = new StringBuilder();
            NBURates.Clear();
            foreach (NBURate rate in _nbuRates)
            {
                // sb.Append($"{rate.txt} {rate.rate}\n");
                NBURates.Add(rate);

            }
            //resultTextBlock.Text = sb.ToString();
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            if(e.OriginalSource is GridViewColumnHeader header)
            {
                if (header.Content is not null && _nbuRates is not null)
                {
                    // MessageBox.Show(header.Content.ToString());
                    NBURates.Clear();
                    List<NBURate> nbuRates = header.Content.ToString() switch
                    {
                        "cc" => _nbuRates.OrderBy(r => r.cc).ToList(),
                        "txt" => _nbuRates.OrderBy(r => r.txt).ToList(),
                        "r030" => _nbuRates.OrderBy(r => r.r030).ToList(),
                        "rate" => _nbuRates.OrderBy(r => r.rate).ToList(),
                        _ => _nbuRates,
                    };
                    foreach (NBURate rate in nbuRates)
                    {
                        // sb.Append($"{rate.txt} {rate.rate}\n");
                        NBURates.Add(rate);

                    }
                }
            }
        }

        private async void NBUday_Click(object sender, RoutedEventArgs e)
        {
            if (ApiDatePicker.SelectedDate is DateTime date)
            {

                String url = $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={date.ToString("yyyyMMdd")}&json";
                
                _nbuRates = await httpClient.GetFromJsonAsync<List<NBURate>>(url);

                if (_nbuRates is null)
                {
                    MessageBox.Show("json err");
                    return;
                };
                //StringBuilder sb = new StringBuilder();
                NBURates.Clear();

                foreach (NBURate rate in _nbuRates)
                {
                    // sb.Append($"{rate.txt} {rate.rate}\n");
                    NBURates.Add(rate);

                }
            }
        }
    }
    public class NBURate
    {
        public int r030 { get; set; }
        public String txt { get; set; }
        public double rate { get; set; }
        public String cc { get; set; }
        public String exchangedate { get; set; }
    }
}
