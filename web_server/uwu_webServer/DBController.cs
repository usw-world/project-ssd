using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;

namespace uwu_webServer {
    public class DBController {
        public DBController() {
            conn = new MySqlConnection(ConnectionContext);
            conn.Open();
        }

        public int UpdateData(string sql) {
            return new MySqlCommand(sql, conn).ExecuteNonQuery();
        }
        public string FindId(string token) {
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
        private void ReadUser(MySqlDataReader reader, JsonObject data) {
            while (reader.Read())
            {
                Console.WriteLine("read user data");
                data.Add("token", reader["user_token"].ToString());
            }
        }
        private void ReadSkill(MySqlDataReader reader, JsonObject data) {
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
            var bits = (length * 6);
            var byte_size = ((bits + 7) / 8);
            var byteArray = new byte[byte_size];
            // crypto.GetBytes(byteArray);
            RandomNumberGenerator.Create().GetBytes(byteArray);
            return Convert.ToBase64String(byteArray);
        }
        public bool SetToken(string token, string userId)
        {
            try
            {
                string sql = $"UPDATE user SET user_token='{token}' WHERE user_id='{userId}';";
                int status = new MySqlCommand(sql, conn).ExecuteNonQuery();
                if(status == 1)
                    Console.WriteLine("토큰 변경 성공");
                else
                    Console.WriteLine("토큰 변경 실패");
                Console.WriteLine(token);
                return true;
            }
            catch (Exception e) {
                Console.Error.WriteLine(e);
                return false;
            }
        }

        public bool SearchUser(string userId, Action<MySqlDataReader> callback) {
            MySqlDataReader? reader = null;
            try {
                var command = new MySqlCommand("SELECT * FROM user WHERE user_id=@user_id", conn);
                command.Parameters.Add(new MySqlParameter("@user_id", userId));
                reader = command.ExecuteReader();
                callback?.Invoke(reader);
                return true;
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
                return false;
            } finally {
                reader?.Close();
            }
        }
        
        public void SearchTable(string sql, Action<MySqlDataReader> callback) {
            MySqlDataReader? reader = null;
            try {
                var command = new MySqlCommand(sql, conn);
                reader = command.ExecuteReader();
                callback?.Invoke(reader);
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
            } finally {
                reader?.Close();
            }
        }
    }
}