using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talkback.Common;
using Talkback.Server.Interfaces;
using System.Data.OleDb;
using System.Text;

namespace Talkback.Server.DAL
{
    public class UsersAccessDal : IDal<Client>
    {
        private OleDbConnection con;
        public UsersAccessDal()
        {
            con = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\TalkbackProject\database.mdb");

        }
        private int ExecuteNonQueryCmd(string query)
        {
            OleDbCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = query;
            cmd.Connection = con;
            var res = cmd.ExecuteNonQuery();
            con.Close();
            return res;
        }
        private bool ExecuteBooleanCmd(string query)
        {
            OleDbCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = query;
            cmd.Connection = con;
            var res = cmd.ExecuteScalar();
            con.Close();
            return res != null;
        }
        private string ExecuteCmd(string query)
        {
            OleDbCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = query;
            cmd.Connection = con;
            var reader = cmd.ExecuteReader();
            string sb = "[";
            int rowNum = 0;
            while (reader.Read())
            {
                if (rowNum > 0)
                    sb += ',';
                var username = reader["username"];
                var password = reader["password"];
                var isonline = reader["isonline"];
                var url = reader["url"];

                sb += $"{{\"UserName\" : \"{username}\", \"Password\" : \"{password}\", \"isOnline\" : \"{isonline}\", \"URL\" : \"{url}\" }}";
                rowNum++;
            }
            con.Close();
            sb += "]";


            return sb.ToString();
        }
        public void Add(Client obj)
        {
            var query = "Insert into Users ([Username],[Password]) Values('" + obj.UserName + "','" + obj.Password + "')";
            ExecuteNonQueryCmd(query);
        }

        public bool Exists(string id, string password)
        {
            var query = $"select * from Users where username='{id}' and password='{password}'";
            return ExecuteBooleanCmd(query);

        }

        public Client Get(string id)
        {
            var query = $"select * from Users where username='{id}'";
            var res = ExecuteCmd(query);
            var clientReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Client>>(res);
            return clientReturned.FirstOrDefault();
        }

        public IEnumerable<Client> GetAll()
        {
            var query = $"select * from Users";
            var res = ExecuteCmd(query);
            var clientReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Client>>(res);
            return clientReturned;
        }

        public bool Remove(Client obj)
        {
            var query = $"delete * from Users where username='{obj.UserName}'";
            return ExecuteNonQueryCmd(query) > 0;
        }

        public bool Remove(string id)
        {
            var query = $"delete * from Users where username='{id}'";
            return ExecuteNonQueryCmd(query) > 0;
        }

        public bool Update(Client obj)
        {
            var query = $"update Users set isonline={obj.IsOnline}, url={obj.URL} where username='{obj.UserName}'";
            return ExecuteNonQueryCmd(query) > 0;
        }

        public bool Update(string id, bool value)
        {
            var query = $"update Users set isonline={value} where username='{id}'";
            return ExecuteNonQueryCmd(query) > 0;
        }
    }
}
