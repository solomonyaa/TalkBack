using System;
using System.Collections.Generic;
using Talkback.Common;

namespace Talkback.Client
{
    public class Token
    {
        private string tokenString;
        private string serverURL;
        private DateTime dueDate;
        private HttpHandler httpHandler;

        public Token(Config config)
        {
            tokenString = "";
            serverURL = config.ServerUrl;
            httpHandler = new HttpHandler();
        }
        public string getToken()
        {
            return this.tokenString;
        }

        public void setToken(string token)
        {
            this.tokenString = token;
        }
        public bool isValid()
        {
            return !(tokenString=="" || DateTime.Now > dueDate);
        }

        public void renewToken(string userName, string password)
        {
            var resp = httpHandler.MakeHttpGet(serverURL, "sign/renewToken", new Dictionary<string, string>()
            {
                { "username", userName },
                { "password",password }
            });

            this.tokenString = resp.Content.ReadAsStringAsync().Result;
            this.dueDate = DateTime.Now.AddYears(1);
        }


    }
}
