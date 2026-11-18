using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Talkback.Common;

namespace Talkback.GUI
{
    public class ViewUpdater
    {
        public delegate void onClientChange(string username, bool isOnline);
        public delegate void onOpponentRollEvent(string cube1, string cube2);
        public delegate void onGameResultEvent(string winner);
        public delegate void onOpponentTurnEvent();

        public static event onClientChange changeClientStatus;
        public static event onOpponentRollEvent opponentRolledDices;
        public static event onGameResultEvent gameResultEvent;
        public static event onOpponentTurnEvent opponentTurnChanged;

        private static IEnumerable<string> onlineClients = new List<string>();

        private HttpHandler HttpHandler;
        public static string clientURL = MainWindow.clientUrl;
        public ViewUpdater()
        {
            HttpHandler = new HttpHandler();
        }

        public void CheckClientsStatus()
        {
            while (true)
            {
                var currentOnline = HttpHandler.HttpGet<IEnumerable<string>>(clientURL, "sign/updateClientsView").ToList();
                var newOfflineClients = onlineClients.Where((c) => currentOnline.Contains(c) == false);
                foreach (var client in newOfflineClients)
                {
                    changeClientStatus?.Invoke(client, false);
                }
                var newOnlineClients = currentOnline.Where((c) => onlineClients.Contains(c) == false);

                foreach (var client in newOnlineClients)
                {
                    changeClientStatus?.Invoke(client, true);
                }
                onlineClients = currentOnline;
                Thread.Sleep(new TimeSpan(0, 0, 5));
            }

        }
        public void ReceiveInGameRequests()
        {
            while (true)
            {
                // update roll result
                var result = HttpHandler.HttpGet<(string, string)>(clientURL, "ingame/ViewRoll");
                opponentRolledDices?.Invoke(result.Item1, result.Item2);

                // update turn
                var turnResult = HttpHandler.MakeHttpGet(clientURL, "ingame/viewTurn");
                if (turnResult.Content.ReadAsStringAsync().Result == "true")
                    opponentTurnChanged?.Invoke();

                // update winner
                turnResult = HttpHandler.MakeHttpGet(clientURL, "ingame/viewGameResult");
                gameResultEvent?.Invoke(turnResult.Content.ReadAsStringAsync().Result);

                // update game status
                var gameResult = HttpHandler.MakeHttpGet(clientURL, "game/updateGameView");
                gameResultEvent?.Invoke(turnResult.Content.ReadAsStringAsync().Result);
                Thread.Sleep(new TimeSpan(0, 0, 5));
            }
        }
    }
}
