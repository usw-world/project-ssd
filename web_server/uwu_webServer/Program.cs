using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MySql.Data.MySqlClient;

namespace uwu_webServer
{
    public class Startup
    {
        private DBController dbc = new DBController();
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            try
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/ping", async (context) => {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        await context.Response.WriteAsync("pong");
                    });
                    #region 회원가입
                    endpoints.MapPost("/register", async (context) => {
                        Console.WriteLine("회원가입");
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8)) {
                            JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);

                            JsonObject answer = new JsonObject();

                            JsonElement idJson;
                            JsonElement pwdJson;

                            if(requestBody.RootElement.TryGetProperty("user_id", out idJson)
                            && requestBody.RootElement.TryGetProperty("user_pw", out pwdJson)) {
                                context.Response.Headers["Content-Type"] = "application/json";
                                
                                string? requestId = idJson.GetString();
                                string? requestPwd = ConvertKR2EN(pwdJson.GetString());
                                if(requestId == null
                                || requestPwd == null
                                || requestId == ""
                                || requestPwd == "") {
                                    answer["message"] = "아이디와 패스워드는 공백일 수 없습니다.";
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync(answer.ToString());
                                    return;
                                }
                                bool alreadyExitId = dbc.SearchUser(requestId);
                                if(!alreadyExitId) {
                                    dbc.InsertUser(requestId, requestPwd);

                                    string token = GenerateToken() + requestId;
                                    dbc.SetToken(token, requestId);

                                    answer["token"] = token;
                                    answer["userId"] = requestId;
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                    await context.Response.WriteAsync(answer.ToString());
                                } else {
                                    answer["message"] = "이미 사용 중인 아이디입니다.";
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync(answer.ToString());
                                }
                            } else {
                                answer["message"] = "올바르지 않은 요청을 수신하였습니다.";
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                
                                await context.Response.WriteAsync(answer.ToString());
                                return;
                            }
                        }
                    });
                    #endregion
                    #region 로그인 
                    endpoints.MapPost("/login", async (context) => {
                        Console.WriteLine("로그인");
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8)) {
                            JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);

                            JsonObject answer = new JsonObject();

                            JsonElement idJson;
                            JsonElement pwdJson;

                            if(requestBody.RootElement.TryGetProperty("user_id", out idJson)
                            && requestBody.RootElement.TryGetProperty("user_pw", out pwdJson)) {
                                context.Response.Headers["Content-Type"] = "application/json";
                                
                                string? requestId = idJson.GetString();
                                string? requestPwd = ConvertKR2EN(pwdJson.GetString());
                                if(requestId == null
                                || requestPwd == null
                                || requestId == ""
                                || requestPwd == "") {
                                    answer["message"] = "아이디와 패스워드는 공백일 수 없습니다.";
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsync(answer.ToString());
                                    return;
                                }
                                string token = "";
                                bool successSearchUser = dbc.SearchUser(requestId, async (MySqlDataReader reader) => {
                                    reader.Read();
                                    string searchedPwd = reader.GetString("user_pw");
                                    if(searchedPwd == requestPwd) {
                                        token = GenerateToken() + requestId;
                                    } else {
                                        answer["message"] = "아이디 또는 패스워드가 일치하지 않습니다.";
                                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                        await context.Response.WriteAsync(answer.ToString());
                                        return;
                                    }
                                });
                                if(successSearchUser) {
                                    dbc.SetToken(token, requestId);
                                    answer["token"] = token;
                                    answer["userId"] = requestId;
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                    // Console.WriteLine(answer);
                                    await context.Response.WriteAsync(answer.ToString());
                                }
                            } else {
                                answer["message"] = "올바르지 않은 요청을 수신하였습니다.";
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                
                                await context.Response.WriteAsync(answer.ToString());
                                return;
                            }
                        }
                    });
                    #endregion
                    
                    #region 업데이트
                    endpoints.MapPost("/update", async (context) => {
                        Console.WriteLine("업데이트");
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8)) {
                            // user_id	        varchar(16)         ||       id       	    varchar(255)
                            // user_pw	        varchar(255)	    ||       t_sklil_data     varchar(255)
                            // user_token	    varchar(255)        ||       q_sklil_data     varchar(255)
                            // t_experience	    int	                ||
                            // t_level	        int	                ||
                            // q_experience	    int 	            ||
                            // q_level	        int	                ||
                            try {
                                string sql = "";
                                JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);
                                JsonElement json = requestBody.RootElement;

                                string? token = json.GetProperty("token").GetString();
                                if(token == null)
                                    throw new Exception("Invalid request exception.");
                                string user_id = dbc.FindId(token);

                                int tExp = json.GetProperty("tExp").GetInt16();
                                int qExp = json.GetProperty("qExp").GetInt16();
                                int tLevel = json.GetProperty("tLevel").GetInt16();
                                int qLevel = json.GetProperty("qLevel").GetInt16();
                                sql = @$"
                                    UPDATE user SET 
                                        t_exp='{tExp}', 
                                        q_exp='{qExp}', 
                                        t_level='{tLevel}',
                                        q_level='{qLevel}'
                                    WHERE 
                                        user_id='{user_id}'
                                ";
                                int result = dbc.UpdateData(sql);
                                if(result < 0)
                                    throw new Exception("Failed update user data.");
                                
                                string tSkillData = json.GetProperty("tSkillData").GetString();
                                string qSkillData = json.GetProperty("qSkillData").GetString();
                                if(tSkillData == null) {
                                    Console.Error.WriteLine("t skill data that received is null.");
                                    tSkillData = "00 00 00 00 00 00";
                                }
                                if(qSkillData == null) {
                                    Console.Error.WriteLine("q skill data that received is null.");
                                    qSkillData = "00 00 00 00 00 00 00";
                                }
                                sql = @$"
                                    UPDATE skill SET 
                                        t_skill_data='{tSkillData}',
                                        q_skill_data='{qSkillData}'
                                    WHERE 
                                        id='{user_id}'
                                ";
                                result = dbc.UpdateData(sql);
                                if(result < 0)
                                    throw new Exception("Failed update skill data.");

                                context.Response.Headers["Content-Type"] = "application/json";
                                context.Response.StatusCode = StatusCodes.Status200OK;
                                await context.Response.WriteAsync("");
                            } catch(Exception e) {
                                Console.Error.WriteLine(e);
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            }
                        }
                    });
                    #endregion
                    
                    #region 데이터 요청
                    endpoints.MapPost("/get-data", async (context) => {
                        Console.WriteLine("데이터 요청");
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                        {
                            var sql = "";
                            var answer = new JsonObject();
                            JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);
                            var token = requestBody.RootElement.GetProperty("token").GetString();
                            var user_id = dbc.FindId(token);
                            sql = $"select * from skill where id = '{user_id}'";
                            (string t, string q) skillDatas = dbc.ReadData();
                            answer["tSkillData"] = token;
                            answer["qSkillData"] = token;
                            context.Response.Headers["Content-Type"] = "application/json";
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            await context.Response.WriteAsync(answer.ToString());
                        }
                    });
                    #endregion 데이터 요청
                });
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }
        public string GenerateToken() {
            var bytes = new byte[4];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(bytes);
            return Convert.ToHexString(bytes);
        }
        private string GetHexFromEnumerator(IEnumerator<JsonElement> enumerator) {
            List<string> result = new List<string>();
            if(enumerator.MoveNext()) {
                result.Add(enumerator.Current.GetInt16().ToString("X"));
            }
            string.Join(" ", result.ToArray());
            return string.Join(" ", result);
        }
        private string? ConvertKR2EN(string? kr) {
            return kr?.Replace('ㅂ', 'q')
            .Replace('ㅈ', 'w')
            .Replace('ㄷ', 'e')
            .Replace('ㄱ', 'r')
            .Replace('ㅅ', 't')
            .Replace('ㅛ', 'y')
            .Replace('ㅕ', 'u')
            .Replace('ㅑ', 'i')
            .Replace('ㅐ', 'o')
            .Replace('ㅔ', 'p')
            .Replace('ㅁ', 'a')
            .Replace('ㄴ', 's')
            .Replace('ㅇ', 'd')
            .Replace('ㄹ', 'f')
            .Replace('ㅎ', 'g')
            .Replace('ㅗ', 'h')
            .Replace('ㅓ', 'j')
            .Replace('ㅏ', 'k')
            .Replace('ㅣ', 'l')
            .Replace('ㅋ', 'z')
            .Replace('ㅌ', 'x')
            .Replace('ㅊ', 'c')
            .Replace('ㅍ', 'v')
            .Replace('ㅠ', 'b')
            .Replace('ㅜ', 'n')
            .Replace('ㅡ', 'm');
        }
    }
    
    public class Program {
        public static void Main(string[] args){
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}