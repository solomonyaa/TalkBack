using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talkback.Common
{
    public class Client
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsOnline { get; set; }
        public string URL { get; set; }
        private string token;
        public Client()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.IsOnline = false;
            this.URL = string.Empty;
        }
        public Client(string JsonString)
        {
            var c = Newtonsoft.Json.JsonConvert.DeserializeObject<Client>(JsonString);
            this.UserName = c.UserName;
            this.Password = c.Password;
            this.IsOnline = c.IsOnline;
            this.URL = c.URL;
        }
        public Client(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
            this.IsOnline = false;
            this.URL = string.Empty;
        }
        public Client(string username, string password, bool isOnline, string url)
        {
            this.UserName = username;
            this.Password = password;
            this.IsOnline = isOnline;
            this.URL = url;
        }
        public string getToken()
        {
            return this.token;
        }
        public void UpdateToken(string token)
        {
            this.token = token;
        }
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
