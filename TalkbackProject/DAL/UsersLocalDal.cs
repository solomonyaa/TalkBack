using System.Collections.Generic;
using Talkback.Common;
using Talkback.Server.Interfaces;

namespace Talkback.Server.DAL
{
    public class UsersLocalDal : IDal<Client>
    {
        private List<Client> clients;
        public UsersLocalDal()
        {
            this.clients = new List<Client>()
            {
                new Client("a","as",true,""),
                new Client("b","bs",true,""),
                new Client("c","cs",true,""),
                new Client("d","ds",false,""),
                new Client("e","es",true,""),
                new Client("f","fs",true,""),
            };
        }
        public void Add(Client obj)
        {
            this.clients.Add(obj);
        }
        public Client Get(string id)
        {
            return this.clients.Find((c) => c.UserName == id);
        }
        public bool Exists(string id, string password)
        {
            return clients.Exists((c) => c.UserName == id && c.Password == password);
        }
        public IEnumerable<Client> GetAll()
        {
            return this.clients;
        }
        public bool Remove(Client obj)
        {
            return this.clients.Remove(obj);
        }
        public bool Remove(string id)
        {
            var client = this.clients.Find((c) => c.UserName == id);
            if (client == null)
                return false;
            return this.clients.Remove(client);
        }
        public bool Update(Client obj)
        {
            var client = this.clients.Find((c) => c.UserName == obj.UserName);
            if (client == null)
                return false;
            client.IsOnline = obj.IsOnline;
            client.URL = obj.URL;
            client.Password = obj.Password;
            return true;
        }
        public bool Update(string id, bool value)
        {
            var client = this.clients.Find((c) => c.UserName == id);
            if (client == null)
                return false;
            client.IsOnline = value;
            return true;
        }
    }
}
