using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using Talkback.Common;

namespace Talkback.Console
{
    public class Program
    {
        public static OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Solomon\source\repos\talkback\TalkbackProject\database.mdb");
        public static int ExecuteNonQueryCmd(string query)
        {
            OleDbCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = query;
            cmd.Connection = con;
            var res = cmd.ExecuteNonQuery();
            con.Close();
            return res;
        }
        public static bool ExecuteBooleanCmd(string query)
        {
            OleDbCommand cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = query;
            cmd.Connection = con;
            var res = cmd.ExecuteScalar();
            con.Close();
            return res !=null;
        }
        public static string ExecuteCmd(string query)
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
                var isonline =  reader["isonline"];
                var url = reader["url"];

                sb += $"{{\"UserName\" : \"{username}\", \"Password\" : \"{password}\", \"isOnline\" : \"{isonline}\", \"URL\" : \"{url}\" }}";
                rowNum++;
            }
            con.Close();
            sb += "]";

            return sb.ToString();
        }

        public static void Main(string[] args)
        {

            var query = $"select * from Users where username='a'";
            //System.Console.WriteLine(ExecuteCmd(query));
            var c = new Client("a", "b");
            System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(c));
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Client>>(ExecuteCmd(query));
        }

        public object Get(string id)
        {
            var query = $"select * from Users where username='{id}'";
            var res = ExecuteCmd(query);
            throw new NotImplementedException();
        }
    
    }
}
