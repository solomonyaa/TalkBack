using System;
using System.Windows;
using Talkback.Common;
using Talkback.Common.Backgammon;

namespace Talkback.GUI
{
    /// <summary>
    /// Interaction logic for GameResponseWindow.xaml
    /// </summary>
    public partial class GameResponseWindow : Window
    {
        private string askingClient;
        private string askedClient;
        private HttpHandler httpHandler;
        private string clientUrl;
        public bool GameCanceled;
        private ContactWindow prevWindow;
        public GameResponseWindow(ContactWindow window)
        {
            InitializeComponent();
            ViewUpdater.gameResultEvent += GotGameRespone;
            this.prevWindow = window;
            this.askedClient = window.displayUsernameLbl.Content.ToString();
            this.askingClient = window.highlightedUser;
            httpHandler = new HttpHandler();
            clientUrl = MainWindow.clientUrl;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           httpHandler.MakeHttpGet(clientUrl, "game/cancel");
           this.DialogResult = false;
            this.Close();
        }

        private void GotGameRespone(string response)
        {
            var res = PlayOptions.Default;
            Enum.TryParse<PlayOptions>(response, out res);

            if (res == PlayOptions.Approved)
            {

                this.DialogResult = true;

            }
            else if (res == PlayOptions.Rejected)
            {
                this.DialogResult = false;
                MessageBox.Show("your request has been rejected");
                
            }
            this.Close();
        }
    }
}
