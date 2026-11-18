using System;
using System.Collections.Generic;
using System.Linq;
using Talkback.Common;
using Talkback.Server.Interfaces;

namespace Talkback.Server.DAL
{
    public class MessageLocalDal : IDal<Message>
    {
        private List<Message> messages;
        public MessageLocalDal()
        {
            this.messages = new List<Message>
            {
                new Message("a","b","hello world"),
                new Message("b","a","hello world"),
                new Message("a","b","hello world"),
                new Message("b","a","world hello"),
                new Message("a","b","hello world"),
                new Message("b","a","world hello"),
                new Message("a","b","hello world"),
                new Message("b","a","world hello")
            };
        }
        public void Add(Message obj)
        {
            this.messages.Add(obj);
        }

        public bool Exists(string id, string password)
        {
            return this.messages.Exists((m) => m.Sender == id && m.Receiver == password);
        }

        public Message Get(string id)
        {
            return this.messages.Where((m) => m.Sender == id).FirstOrDefault();

        }

        public IEnumerable<Message> GetAll()
        {
            return messages;
        }

        public bool Remove(Message obj)
        {
            return messages.Remove(obj);
        }

        public bool Remove(string id)
        {
            var msgs = this.messages.FindAll((m) => m.Sender == id);
            var removedAll = true;
            foreach (var m in msgs)
            {
                removedAll = removedAll && this.messages.Remove(m);
            }
            return removedAll;
        }

        public bool Update(Message obj)
        {
            throw new NotImplementedException();
        }

        public bool Update(string id, bool value)
        {
            throw new NotImplementedException();
        }
    }
}
