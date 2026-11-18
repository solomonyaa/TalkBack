using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Talkback.Client
{
    public class ClientState
    {
        public Token token;
        private Dictionary<string, bool> currentOnline;

        public ClientState(Token token)
        {
            this.token = token;
            currentOnline = new Dictionary<string, bool>();
        }
        public void SetClientState(string username, bool isOnline)
        {
            if (!currentOnline.Keys.Contains(username))
                this.currentOnline.Add(username, isOnline);
            else
                this.currentOnline[username] = isOnline;
        }
        public IEnumerable<string> GetOnlineClients()
        {
            return this.currentOnline.Where((kv) => kv.Value == true).Select((kv) => kv.Key);
        }
    }
}
