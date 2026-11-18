using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talkback.Common;
using Talkback.Server.Interfaces;

namespace Talkback.Server.DAL
{
    public class MessageAccessDal : IDal<Message>
    {
        public void Add(Message obj)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string id, string password)
        {
            throw new NotImplementedException();
        }

        public Message Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Remove(Message obj)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string id)
        {
            throw new NotImplementedException();
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
