using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talkback.Common.Backgammon
{
    public class Move
    {
        public int previousRow { get; set; }
        public int previousCol { get; set; }
        public int currentRow { get; set; }
        public int CurrentCol { get; set; }
        public bool isWhite { get; set; }

        public Move(string jsonString)
        {
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<Move>(jsonString);
            previousCol = m.previousCol;
            previousRow = m.previousRow;
            currentRow = m.currentRow;
            CurrentCol = m.CurrentCol;
            isWhite = m.isWhite;
        }
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);

        }
    }
}
