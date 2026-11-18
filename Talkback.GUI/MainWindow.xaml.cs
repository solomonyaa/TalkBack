using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Talkback.Common;

namespace Talkback.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpHandler httpHandler;
        public static string clientUrl;

        public MainWindow()
        {

        }
        public MainWindow(string clientURL)
        {
            InitializeComponent();

            clientUrl = clientURL;
            httpHandler = new HttpHandler();
        }

        private async void RunUpdates()
        {
            ViewUpdater vu = new ViewUpdater();

            await Task.Factory.StartNew(() => vu.CheckClientsStatus());
            await Task.Factory.StartNew(() => vu.ReceiveInGameRequests());
        }

        private void SignedIn()
        {
            var res = httpHandler.HttpGet<IEnumerable<Common.Client>>(clientUrl, "sign/signin");

            if (res == null)
            {
                MessageBox.Show("error sign-in", "Error Sign-in", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var dict = res.ToDictionary(x => x.UserName, x => x.IsOnline);
            var contactpage = new ContactWindow(userNameTB.Text, dict);
            contactpage.Show();
            this.Hide();
            contactpage.Closed += Contactpage_Closed;
            connectlbl.Content = "Disconnect";
            actionTB.Visibility = msgLbl.Visibility = backgammonLbl.Visibility = Visibility.Visible;
            passwordTB.Password = userNameTB.Text = "";
            signInSP.Visibility = Visibility.Collapsed;


        }

        private void Contactpage_Closed(object sender, EventArgs e)
        {
            this.Show();
        }

        private void SignedOff()
        {
            connectlbl.Content = "Connect";
            actionTB.Visibility = msgLbl.Visibility = backgammonLbl.Visibility = Visibility.Hidden;
            signInSP.Visibility = Visibility.Visible ;

        }


        private void signBtn_Click(object sender, RoutedEventArgs e)
        {

            var res = this.httpHandler.MakeHttpGet(clientUrl, "sign/exist", new Dictionary<string, string>()
            {
                {"username",userNameTB.Text },
                {"password",passwordTB.Password }

            });
            if (!res.IsSuccessStatusCode)
            {
                MessageBox.Show(res.ReasonPhrase, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var exists = res.Content.ReadAsStringAsync().Result;

            if (exists == "true")
            {
                httpHandler.MakeHttpGet(clientUrl, "sign/renewToken", new Dictionary<string, string>()
            {
                 {"username", userNameTB.Text },
                 {"password" , passwordTB.Password}
            });
                SignedIn();
            }
            else
            {
                var resp = this.httpHandler.MakeHttpGet(clientUrl, "sign/signup", new Dictionary<string, string>()
                {
                    { "username", userNameTB.Text },
                    {"password", passwordTB.Password }
                });
                if (resp.IsSuccessStatusCode)
                    SignedIn();

                MessageBox.Show(resp.ReasonPhrase, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }


        }

        private void CredentialsEntered(object sender, RoutedEventArgs e)
        {
            signBtn.IsEnabled = passwordTB.Password.Length > 0 && userNameTB.Text.Length > 0;
        }

        private void SignHandler(object sender, MouseButtonEventArgs e)
        {
            SignedOff();
        }

        private void msgLbl_clicked(object sender, MouseButtonEventArgs e)
        {

        }

        private void backgammonLbl_clicked(object sender, MouseButtonEventArgs e)
        {

        }

        private void aboutLbl_clicked(object sender, MouseButtonEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            MessageBox.Show("Talkback is blash blah", "About", button, icon);
        }

        private void htpLbl_clicked(object sender, MouseButtonEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            var tutorial = File.ReadAllText(@"BackgammonUtils\Tutorial.txt");
            MessageBox.Show(tutorial, "About", button, icon);
        }

        private void menuBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            menuSP.Visibility = menuSP.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        }
    }
}
