using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talkback.Common.Backgammon;

namespace Talkback.Client
{
    public class GameState
    {
        public PlayOptions currentState { get; set; }
        public Token Token { get; set; }
        public bool CanPlay { get; set; }
        public (string, string) CurrentCubes { get; set; }
        public string GameWinner { get; set; }
    }
}
