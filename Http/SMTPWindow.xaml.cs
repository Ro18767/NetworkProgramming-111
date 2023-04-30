using Http.Data;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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

namespace Http
{
    /// <summary>
    /// Interaction logic for SMTPWindow.xaml
    /// </summary>
    public partial class SMTPWindow : Window
    {
        private Random random = new Random();
        
        public SMTPWindow()
        {
            InitializeComponent();
        }
        private dynamic? emailConfig;
        private String emailTemplate;
        private readonly DataContext dataContext = new DataContext();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String configFilename = "emailconfig.json";
            try
            {
                emailConfig = JsonSerializer.Deserialize<dynamic>(
                    System.IO.File.ReadAllText(configFilename)
                );
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Не найден файл конфигурации {configFilename}");
                this.Close();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Ошибка преобразования конфигурации: {ex.Message}");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки конфигурации: {ex.Message}");
                this.Close();
            }
            String emailTeplateFilename = "email.html";
            try
            {
                emailTemplate = System.IO.File.ReadAllText(emailTeplateFilename);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Не найден файл шаблона {emailTeplateFilename}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обработки шаблона: {ex.Message}");
                this.Close();
            }
            NpUser? user = dataContext.NpUsers.FirstOrDefault(u => u.Name == UserNameTextBox.Text && u.Email == UserEmailTextBox.Text);
            
            if (user is null)
            {
                ConfirmDockPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConfirmDockPanel.Visibility = Visibility.Visible;
                RegisterButton.IsEnabled = false;
            }
            
            
        }

        private void SendTestButton_Click(object sender, RoutedEventArgs e)
        {
            if(emailConfig is null) return;
            /* 
             * emailConfig.GetProperty("smtp").GetProperty("gmail").GetProperty("host").GetString();
             */

            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            String host = gmail.GetProperty("host").GetString()!;
            int port = gmail.GetProperty("port").GetInt32()!;
            String mailbox = gmail.GetProperty("email").GetString()!;
            String password = gmail.GetProperty("password").GetString()!;
            bool ssl = gmail.GetProperty("ssl").GetBoolean()!;

            using SmtpClient smtpClient = new(host) {
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password),
            };
            try
            {
                smtpClient.Send(
                from: mailbox,
                recipients: "timaropw@gmail.com",
                subject: "Test message",
                body: "Test message smtpWindow"
                );
                MessageBox.Show("Send OK");
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Send Err: {ex.Message}");
            }
        }

        private void SendHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            if (emailConfig is null) return;
            /* 
             * emailConfig.GetProperty("smtp").GetProperty("gmail").GetProperty("host").GetString();
             */

            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            String host = gmail.GetProperty("host").GetString()!;
            int port = gmail.GetProperty("port").GetInt32()!;
            String mailbox = gmail.GetProperty("email").GetString()!;
            String password = gmail.GetProperty("password").GetString()!;
            bool ssl = gmail.GetProperty("ssl").GetBoolean()!;

            using SmtpClient smtpClient = new(host)
            {
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password),
            };
            try
            {
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(mailbox),
                    Body = "<u>Test</u> <i>message</i> from <b style='color:green'>SmtpWindow</b>",
                    IsBodyHtml = true,
                    Subject = "Test message"
                };
                mailMessage.To.Add(new MailAddress("timaropw@gmail.com"));
                smtpClient.Send(mailMessage);
                MessageBox.Show("Send OK");
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Send Err: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

            if (emailConfig is null) return;
            /* 
             * emailConfig.GetProperty("smtp").GetProperty("gmail").GetProperty("host").GetString();
             */

            JsonElement gmail = emailConfig.GetProperty("smtp").GetProperty("gmail");
            String host = gmail.GetProperty("host").GetString()!;
            int port = gmail.GetProperty("port").GetInt32()!;
            String mailbox = gmail.GetProperty("email").GetString()!;
            String password = gmail.GetProperty("password").GetString()!;
            bool ssl = gmail.GetProperty("ssl").GetBoolean()!;


            using SmtpClient smtpClient = new(host)
            {
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password),
            };

            String confirmCode = $"{random.Next(0, 10)}{random.Next(0, 10)}{random.Next(0, 10)}{random.Next(0, 10)}{random.Next(0, 10)}{random.Next(0, 10)}";

            var emailMessage = emailTemplate
                .Replace("%name%", UserNameTextBox.Text)
                .Replace("%code%", confirmCode);

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(mailbox),
                Body = emailMessage,
                IsBodyHtml = true,
                Subject = "Code For Confirm"
            };
            mailMessage.To.Add(new MailAddress(UserEmailTextBox.Text));
            smtpClient.Send(mailMessage);
            dataContext.NpUsers.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = UserNameTextBox.Text,
                Email = UserEmailTextBox.Text,
                Code = confirmCode,
            });
            dataContext.SaveChanges();
            ConfirmDockPanel.Visibility = Visibility.Visible;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
