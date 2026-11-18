using System.Collections.Generic;

namespace Talkback.Server.Interfaces
{
    public interface IDal<T>
    {
        void Add(T obj);
        T Get(string id);
        IEnumerable<T> GetAll();
        bool Update(T obj);
        bool Update(string id, bool value);
        bool Remove(T obj);
        bool Remove(string id);
        bool Exists(string id, string password);
    }
}
