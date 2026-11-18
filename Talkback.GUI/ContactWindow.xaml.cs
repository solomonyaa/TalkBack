using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Talkback.Common;
using Talkback.Common.Backgammon;

namespace Talkback.GUI
{
    /// <summary>
    /// Interaction logic for ContactWindow.xaml
    /// </summary>
    public partial class ContactWindow : Window
    {
        public delegate void OnHighlightedChanged(string user);
        public event OnHighlightedChanged HighlightedUserchanged;

        public string highlightedUser = string.Empty;
        private HttpHandler httpHandler;
        private string ClientURL;
        public static Dictionary<string, bool> clientsStatus;


        public ContactWindow(string signedInUser, Dictionary<string, bool> signedClients)
        {
            InitializeComponent();
            clientsStatus = signedClients;
            httpHandler = new HttpHandler();
            ClientURL = MainWindow.clientUrl;

            HighlightedUserchanged += ChangeChatPane;
            ViewUpdater.changeClientStatus += ViewUpdater_changeClientStatus;
            displayUsernameLbl.Content = signedInUser;
            populateClientsSP();

        }
        private void populateClientsSP()
        {
            OnlineUsersLB.Items.Clear();
            OfflineUsersLB.Items.Clear();
            foreach (var client in clientsStatus)
            {
                if (client.Key == displayUsernameLbl.Content.ToString())
                    continue;

                StackPanel sp = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                sp.Children.Add(new Image() { Height = 15 });
                sp.Children.Add(new Label() { FontSize = 12, Content = client.Key });
                var lbItem = new ListBoxItem
                {
                    Content = sp,

                };

                if (client.Value) // client is online
                {

                    ((Image)sp.Children[0]).Source = getImage(@"images\online_indicator.png");
                    lbItem.Selected += ClientSelected;
                    lbItem.LostFocus += LbItem_LostFocus;
                    OnlineUsersLB.Items.Add(lbItem);
                }
                else
                {
                    ((Image)sp.Children[0]).Source = getImage(@"images\offline_indicator.png");
                    lbItem.Selected += OfflineClientSelected;
                    OfflineUsersLB.Items.Add(lbItem);
                }

            }
        }
        private void ViewUpdater_changeClientStatus(string username, bool isOnline)
        {
            clientsStatus[username] = isOnline;
            populateClientsSP();
        }

        private void OfflineClientSelected(object sender, RoutedEventArgs e)
        {
            highlightedUser = "offline";

            HighlightedUserchanged?.Invoke(highlightedUser);
        }

        private void ChangeChatPane(string user)
        {
            ClearMsgPane();
            PopulateMsg();
        }

        private BitmapImage getImage(string path)
        {

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path, UriKind.Relative);
            image.EndInit();
            return image;

        }
        private void LbItem_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void ClearMsgPane()
        {
            if (highlightedUser == string.Empty)
                return;
            this.msgSP.Items.Clear();
        }

        private void ClientSelected(object sender, RoutedEventArgs e)
        {
            highlightedUser = ((Label)((StackPanel)((ListBoxItem)e.Source).Content).Children[1]).Content.ToString();
            HighlightedUserchanged?.Invoke(highlightedUser);

            SendMsgTB.Text = highlightedUser;
        }

        private void backgammonImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (highlightedUser == string.Empty)
                //computerized opponent. not implemented
                return;

            var resp = httpHandler.MakeHttpGet(ClientURL, "game/play", new Dictionary<string, string>()
            {
                {"oponnentUserName",highlightedUser }
            });
            GameResponseWindow grw = new GameResponseWindow(this);
            var showBackgmmonWindow = true;

            if (showBackgmmonWindow == true)
            {
                var BackgammonWindow = new BackgammonWindow(displayUsernameLbl.Content.ToString(), highlightedUser);
                BackgammonWindow.Show();
                this.Hide();

                BackgammonWindow.Closed += BackgammonWindow_StateChanged;
            }


        }

        private void BackgammonWindow_StateChanged(object sender, EventArgs e)
        {
            this.Show();
        }

        private void PopulateMsg()
        {
            if (highlightedUser == string.Empty || highlightedUser == "offline")
                return;
            var messages = httpHandler.HttpGet<IEnumerable<Message>>(ClientURL, "message/getmessages", new Dictionary<string, string>()
            {
                {"otherClient",highlightedUser }
            }).ToList();

            for (int i = 0; i < messages.Count; i++)
            {
                HorizontalAlignment currentAlignment = i % 2 == 0 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                var lvi = new ListViewItem() { HorizontalAlignment = currentAlignment };

                var sp = new StackPanel() { Width = 200 };
                var border = new Border() { HorizontalAlignment = currentAlignment, BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
                sp.Children.Add(new Label() { Content = messages[i].Sender + " " + messages[i].Date, HorizontalAlignment = HorizontalAlignment.Right });
                sp.Children.Add(new TextBlock() { Text = messages[i].Content, FontSize = 15, TextWrapping = TextWrapping.WrapWithOverflow, HorizontalAlignment = currentAlignment });
                border.Child = sp;
                lvi.Content = border;
                msgSP.Items.Add(lvi);

            }


        }

        private void collapseTriangle2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OfflineUsersLB.Visibility = OfflineUsersLB.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void collapseTriangle1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnlineUsersLB.Visibility = OnlineUsersLB.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void SendMsgTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SendMsgBtn.IsEnabled = SendMsgTB.Text.Length > 0;

        }

        private void SendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            httpHandler.MakeHttpGet(ClientURL, "message/sendmessage", new Dictionary<string, string>()
        {
            {"otherClient", highlightedUser },
            {"message", SendMsgTB.Text}
        });
            PopulateMsg();
            SendMsgTB.Text = "";

        }

        private void chatImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sendMsgPaneSP.Visibility = sendMsgPaneSP.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
