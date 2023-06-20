using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;

namespace uwu_webServer {
    public class DBController {
        public DBController() {
            conn = new MySqlConnection(_connectionContext);
            conn.Open();
        }

        public int InsertUser(string userId, string userPw) {
            string userInsertSql = "INSERT INTO uwu.user (user_id, user_pw) VALUES(@userId, @userPw)";
            var userCommand = new MySqlCommand(userInsertSql, conn);
            userCommand.Parameters.Add(new MySqlParameter("@userId", userId));
            userCommand.Parameters.Add(new MySqlParameter("@userPw", userPw));

            string skillDataInsertSql = "INSERT INTO uwu.skill (id, t_skill_data, q_skill_data) VALUES(@userId, @tSkillData, @qSkillData)";
            var skillDataCommand = new MySqlCommand(skillDataInsertSql, conn);
            skillDataCommand.Parameters.Add(new MySqlParameter("@userId", userId));
            skillDataCommand.Parameters.Add(new MySqlParameter("@tSkillData", "00 00 00 00 00 00"));
            skillDataCommand.Parameters.Add(new MySqlParameter("@qSkillData", "00 00 00 00 00 00 00"));
            if(userCommand.ExecuteNonQuery() >= 0
            && skillDataCommand.ExecuteNonQuery() >= 0) {
                return -1;
            } else {
                return 0;
            }
        }
        public int UpdateData(string sql) {
            return new MySqlCommand(sql, conn).ExecuteNonQuery();
        }
        public string FindId(string token) {
            var answer = "";
            var sql = $"select user_id from user where user_token = '{token}';";
            var reader = new MySqlCommand(sql, conn).ExecuteReader();
            while (reader.Read()) {
                answer = reader["user_id"].ToString();
            }
            reader.Close();
            return answer;
        }
        private void ReadUser(MySqlDataReader reader, JsonObject data) {
            while (reader.Read()) {
                Console.WriteLine("read user data");
                data.Add("token", reader["user_token"].ToString());
            }
        }
        private (string t, string q) ReadSkill(MySqlDataReader reader, JsonObject data) {
            (string t, string q) result = ("", "");
            if(reader.Read()) {
                // result.t += 
                data.Add("skillPoint", reader["skillPoint"].ToString());
                data.Add("skill_UnityBall", reader["skill_UnityBall"].ToString());
                // answer.Add("user_id", $"{reader["id"]}");
                Console.WriteLine("SkillData search");
            }
            return result;
        }
        
        
        private MySqlConnection conn;
        private string _connectionContext = DB_URL.url;

        public bool SetToken(string token, string userId) {
            try {
                string sql = $"UPDATE user SET user_token='{token}' WHERE user_id='{userId}';";
                var command = new MySqlCommand("UPDATE user SET user_token=@token WHERE user_id=@userId");
                command.Parameters.Add(new MySqlParameter("@token", token));
                command.Parameters.Add(new MySqlParameter("@userId", userId));
                int status = new MySqlCommand(sql, conn).ExecuteNonQuery();
                // if(status == 1)
                //     Console.WriteLine("토큰 변경 성공");
                // else
                //     Console.WriteLine("토큰 변경 실패");
                // Console.WriteLine(token);
                return true;
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return false;
            }
        }

        public bool SearchUser(string userId, Action<MySqlDataReader>? callback=null) {
            MySqlDataReader? reader = null;
            try {
                var command = new MySqlCommand("SELECT * FROM user WHERE user_id=@user_id", conn);
                command.Parameters.Add(new MySqlParameter("@user_id", userId));
                reader = command.ExecuteReader();
                if(!reader.HasRows)
                    throw new System.Exception("There is no data that is consistent in table.");
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