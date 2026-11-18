using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Talkback.Common;
using Talkback.GUI.BackgammonUtils;

namespace Talkback.GUI
{
    public delegate void SpikeClicked(int row, int column, int cube1, int cube2);
    public delegate void CubesRolled();

    public partial class BackgammonWindow
    {
        private int cube1;
        private int cube2;
        private Board LogicBoard;
        private UiSpikeElement[,] UiBoard;

        private HttpHandler httpHandler;
        private string ClientURL;

        public static event SpikeClicked OnSpikeClicked;
        public static event CubesRolled OnRolled;

        private string hostPlayer;
        private string remotePlayer;
        private Timer timer; 

        public BackgammonWindow(string hostPlayer, string remotePlayer)
        {
            InitializeComponent();

            ViewUpdater.opponentRolledDices += ViewUpdater_opponentRolledDices;
            ViewUpdater.opponentTurnChanged += ViewUpdater_opponentTurnChanged;
            ViewUpdater.gameResultEvent += ViewUpdater_gameResultEvent;
            this.hostPlayer = hostPlayer;
            this.remotePlayer = remotePlayer;

            this.cnctNameLbl.Text = remotePlayer;

            this.timer = new Timer(120000); // set for two minutes;
            this.timer.Elapsed += Timer_Elapsed;
            this.ClientURL = MainWindow.clientUrl;
            httpHandler = new HttpHandler();
            LogicBoard = new Board();
            UiBoard = new UiSpikeElement[2, 12];
            Board.RollCubeEvent += BoardOnRollCubeEvent; //roll cuve at the end of turn

           
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Spike spike = LogicBoard[i, j];
                    UiBoard[i, j] = new UiSpikeElement(i, j);
                    UiBoard[i, j].MouseDown += OnMouseDown;
                    SetInGrid(UiBoard[i, j], i, j);
                    UiBoard[i, j].Update(spike);
                }
            }
        }

        private void ViewUpdater_gameResultEvent(string winner)
        {
            gameResultTB.Text = winner == hostPlayer ? "You Won!" : "You Lost!";
            gameResultTB.Visibility = Visibility.Visible;
            BackgammonBoard.IsEnabled = false;
        }

        private void ViewUpdater_opponentTurnChanged()
        {
            LogicBoard.BlackTurn = !LogicBoard.BlackTurn;
            Update();
        }

        private void ViewUpdater_opponentRolledDices(string cube1, string cube2)
        {
            if (cube1 == TextCube1.Content.ToString() && cube2 == TextCube2.Content.ToString())
            {
                return;
            }
            TextCube1.Content = cube1;
            TextCube2.Content = cube2;
            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // send winning message to turn.content.tostring()
            // send losing message to other participant
        }

        private (int cub1, int cub2) BoardOnRollCubeEvent()
        {
            this.RollBtn.IsEnabled = true;

            //Cubes_Clicked(null, null);
            httpHandler.MakeHttpGet(ClientURL, "ingame/passTurn", new Dictionary<string, string>()
            {
                { "opponentName", remotePlayer } });
            return (cube1, cube2);
        }

        private void SetInGrid(UiSpikeElement element, int row, int column)
        {
            if (column > 5)
            {
                column++;
            }
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            BoardGrid.Children.Add(element);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            UiSpikeElement uiSpikeElement = (UiSpikeElement)sender;
            OnSpikeClicked?.Invoke(uiSpikeElement.Row, uiSpikeElement.Column, cube1, cube2);
            Update();
        }

        private BitmapImage getImage(string path)
        {

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path, UriKind.Relative);
            image.EndInit();
            return image;

        }

        private void Update()
        {
            // WHITE is Host, Black is REMOTE
            Turn.Content = LogicBoard.BlackTurn ? remotePlayer : hostPlayer;
            //RollBtn.IsEnabled = BackgammonBoard.IsEnabled = Turn.Content.ToString() == hostPlayer;
            string path = $"BackgammonUtils/Images/{(LogicBoard.BlackTurn ? "Black" : "White")}Player.png";

            currentPlayerColor.Source = getImage(path);

            if (LogicBoard.blacksOut == 15)
            {
                httpHandler.MakeHttpGet(ClientURL, "ingame/sendGameResult", new Dictionary<string, string>()
            {
                { "winnerName", remotePlayer } });


                gameResultTB.Text = "You Lost!";
                gameResultTB.Visibility = Visibility.Visible;
                //Close();
            }
            if (LogicBoard.whitesOut == 15)
            {
                httpHandler.MakeHttpGet(ClientURL, "ingame/sendGameResult", new Dictionary<string, string>()
            {
                { "winnerName", hostPlayer } });

                gameResultTB.Text = "You Won!";
                gameResultTB.Visibility = Visibility.Visible;
                //Close();
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    UiBoard[i, j].Update(LogicBoard[i, j]);
                }
            }
        }

        private void Cubes_Clicked(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            Random cube = new Random();
            cube1 = cube.Next(1, 7);
            cube2 = cube.Next(1, 7);
            TextCube1.Content = cube1;
            TextCube2.Content = cube2;
            OnRolled?.Invoke();
            this.timer.Start();
            this.httpHandler.MakeHttpGet(ClientURL, "ingame/passRoll", new Dictionary<string, string>()
            {
                {"opponentName",this.remotePlayer },
                {"cube1", cube1.ToString() },
                {"cube2",cube2.ToString() }
            });
            this.RollBtn.IsEnabled = false;

        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("System checking who starts");

            do
            {
                Cubes_Clicked(null, null);
            } while (cube1 == cube2);

            MessageBox.Show(cube1 > cube2 ? $"{remotePlayer} starts" : $"{hostPlayer} starts");
            LogicBoard.BlackTurn = cube1 > cube2;
            Turn.Content = LogicBoard.BlackTurn ? remotePlayer : hostPlayer;
            string path = $"BackgammonUtils/Images/{(LogicBoard.BlackTurn ? "Black" : "White")}Player.png";

            currentPlayerColor.Source = getImage(path);

            onlinePlayersCB.ItemsSource = ContactWindow.clientsStatus.Where((k) => k.Value == true).Select(x => x.Key);
        }

        private void PopulateMsg(string playerName)
        {
            if (playerName == string.Empty )
                return;
            var messages = httpHandler.HttpGet<IEnumerable<Message>>(ClientURL, "message/getmessages", new Dictionary<string, string>()
            {
                {"otherClient",playerName }
            }).ToList();

            for (int i = 0; i < messages.Count; i++)
            {
                HorizontalAlignment currentAlignment = i % 2 == 0 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                var lvi = new ListViewItem() { HorizontalAlignment = currentAlignment };

                var sp = new StackPanel();
                var border = new Border() { HorizontalAlignment = currentAlignment, BorderThickness = new Thickness(1), BorderBrush = Brushes.Black };
                sp.Children.Add(new Label() { Content = messages[i].Sender + " " + messages[i].Date, HorizontalAlignment = HorizontalAlignment.Right });
                sp.Children.Add(new TextBlock() { Text = messages[i].Content, FontSize = 10, TextWrapping = TextWrapping.WrapWithOverflow, HorizontalAlignment = currentAlignment });
                border.Child = sp;
                lvi.Content = border;
                msgSP.Items.Add(lvi);

            }


        }


        private void onlinePlayersCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateMsg(onlinePlayersCB.SelectedItem.ToString());
        }

        private void SendMsgTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SendMsgBtn.IsEnabled = SendMsgTB.Text.Length > 0;
        }
        private void SendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            httpHandler.MakeHttpGet(ClientURL, "message/sendmessage", new Dictionary<string, string>()
        {
            {"otherClient", onlinePlayersCB.SelectedItem.ToString()},
            {"message", SendMsgTB.Text}
        });
            PopulateMsg(onlinePlayersCB.SelectedItem.ToString());
            SendMsgTB.Text = "";
        }

        private void forfeitBtn_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Are tou sure you want to resign?", "resignation confirmation",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (res == MessageBoxResult.OK)
            {
                // show loser screen and send winning screen to remotePlayer
            }
            return;

        }
    }


}
