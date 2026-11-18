using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talkback.Common
{
    public class Message
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }

        public Message()
        {

        }
        public Message(string sender, string receiver, string content)
        {
            Sender = sender;
            Receiver = receiver;
            Content = content;
            Date = DateTime.Now.ToString();
        }
        public Message(string jsonString)
        {
            var m = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(jsonString);

            this.Sender = m.Sender;
            this.Receiver = m.Receiver;
            this.Content = m.Content;
            this.Date = m.Date;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
