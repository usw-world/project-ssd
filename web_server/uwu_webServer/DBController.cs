using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;

namespace uwu_webServer
{
    public class DBController
    {
        public DBController()
        {
            conn = new MySqlConnection(ConnectionContext);
            conn.Open();
        }

        public int UpdateData(string sql)
        {
            return new MySqlCommand(sql, conn).ExecuteNonQuery();
        }

        public string FindId(string token)
        {
            var answer = "";
            var sql = $"select user_id from user where user_token = '{token}';";
            var reader = new MySqlCommand(sql, conn).ExecuteReader();
            while (reader.Read())
            {
                answer = reader["user_id"].ToString();
            }
            reader.Close();
            return answer;
        }
        public void ReadUser(MySqlDataReader reader, JsonObject data)
        {
            while (reader.Read())
            {
                Console.WriteLine("read user data");
                data.Add("token", reader["user_token"].ToString());
            }
        }

        private void ReadSkill(MySqlDataReader reader, JsonObject data)
        {
            while (reader.Read())
            {
                data.Add("skillPoint", reader["skillPoint"].ToString());
                data.Add("skill_UnityBall", reader["skill_UnityBall"].ToString());
                // answer.Add("user_id", $"{reader["id"]}");
                Console.WriteLine("SkillData search");
            }
        }
        
        
        private MySqlConnection conn;
        private const string ConnectionContext = "Server=pjr-uwu.mysql.database.azure.com;UserID = hwan;Password=truelove08!;Database=uwu;";

        public string GenerateToken(int length)
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                var bits = (length * 6);
                var byte_size = ((bits + 7) / 8);
                var byteArray = new byte[byte_size];
                crypto.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray);
            }
        }
        public string SetToken(string user_id)
        {
            var token = "";
            bool flag;
            string sql;
            try
            {
                do
                {
                    flag = false;
                    sql = $"select * from user where user_token = '{token}';";
                    token = GenerateToken(8);
                    Console.WriteLine("token value change");
                    var reader = new MySqlCommand(sql, conn).ExecuteReader();
                    while (reader.Read())
                    {
                        flag = true;
                    }
                    reader.Close();
                } while (flag);
                Console.WriteLine("test End token : "+token);
                sql = $"update user set user_token = '{token}' where user_id = '{user_id}';";
                int status = new MySqlCommand(sql, conn).ExecuteNonQuery();
                if(status == 1)
                    Console.WriteLine("토큰 변경 성공");
                else
                    Console.WriteLine("토큰 변경 실패");
                Console.WriteLine(token);
                return token+"";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return "Error";
        }
        
        public void SearchTable(string sql, string type, JsonObject data)
        {
            try
            {
                var command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = command.ExecuteReader();
                switch (type)
                {
                    case "user":
                        ReadUser(reader, data);
                        break;  
                    case "skill":
                        ReadSkill(reader, data);
                        break;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        
        
    }
}