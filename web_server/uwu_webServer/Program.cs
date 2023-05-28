using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

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
                    #region 회원가입
                    endpoints.MapPost("/register", async context =>
                    {
                        JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);
                        string user_id = requestBody.RootElement.GetProperty("user_id").GetString();
                        Console.WriteLine(user_id);
                        string user_pw = requestBody.RootElement.GetProperty("user_pw").GetString();
                        var sql = $"select * from user where user_id = '{user_id}'";
                        var value = new JsonObject(); 
                        dbc.SearchTable(sql, "user", value);
                        Console.WriteLine(value.ToString());
                        
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        
                        if (value.ToString().Equals("{}"))
                        {
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            int status;
                            sql = $"insert into user(user_id, user_pw) values ('{user_id}', '{user_pw}');";
                            dbc.UpdateData(sql);
                            sql = $"insert into skill(id) value('{user_id}');";
                            status = dbc.UpdateData(sql);
                            if (status != 1)
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        }
                        context.Response.Headers["Content-Type"] = "application/json";
                        await context.Response.WriteAsync(value.ToString());
                    });
                    #endregion
                    
                    endpoints.MapPost("/user", async context =>
                    {
                        var sql = "select * from user";
                        var value = new JsonObject();
                        dbc.SearchTable(sql, "user", value);
                        if (value == null)
                            return;
                        context.Response.Headers["Content-Type"] = "application/json";
                        await context.Response.WriteAsync(value.ToString());
                    });
                    // endpoints.Map("/", HandlePostRequest);
                    endpoints.MapPost("/skill", async context =>
                    {
                        var sql = "select * from skill";

                        JsonObject value = new JsonObject();
                        dbc.SearchTable(sql, "skill", value);
                        if (value == null)
                            return;
                        context.Response.Headers["Content-Type"] = "application/json";
                        await context.Response.WriteAsync(value.ToString());
                    });
                    
                    #region 로그인 
                    endpoints.MapPost("/login", async context =>
                    {

                        // 요청 바디 파싱
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                        {
                            JsonObject answer = new JsonObject();
                            JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);
                            string user_id = requestBody.RootElement.GetProperty("user_id").GetString();
                            string password = requestBody.RootElement.GetProperty("user_pw").GetString();
                            Console.WriteLine(user_id+" : "+password);
                            string sql = $"select * from user where user_id = '{user_id}'";
                            dbc.SearchTable(sql, "user", answer);
                            if (answer.ToString().Equals("{}"))
                            {
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                Console.WriteLine("아이디 확인");
                                return;
                            }
                            
                            if (answer["user_pw"].ToString().Equals(password))
                            {
                                sql = $"select * from skill where id = '{user_id}'";
                                string token = dbc.SetToken(answer["user_id"].ToString());
                                dbc.SearchTable(sql, "skill", answer);
                                answer["token"] = token;
                                Console.WriteLine(answer.ToString());
                                context.Response.Headers["Content-Type"] = "application/json";
                                context.Response.StatusCode = StatusCodes.Status200OK;
                                await context.Response.WriteAsync(answer.ToString());
                            }
                            else
                            {
                                Console.WriteLine("비밀번호 확인");
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            }
                        }
                    });
                    #endregion
                    
                    #region 업데이트
                    endpoints.MapPost("/update", async context =>
                    {
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                        {
                            var sql = "";
                            JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);
                            var token = requestBody.RootElement.GetProperty("token").GetString();
                            var user_id = dbc.FindId(token);
                            var skillPoint = requestBody.RootElement.GetProperty("skillPoint").GetString();
                            var skill_UnityBall = requestBody.RootElement.GetProperty("skill_UnityBall");
                            Console.WriteLine(token+" / "+user_id+" / "+skillPoint+" / "+skill_UnityBall);
                            sql = $"update skill set skillPoint = '{skillPoint}' where id = '{user_id}'";
                            dbc.UpdateData(sql);
                            sql = $"update skill set skill_UnityBall = '{skill_UnityBall}' where id = '{user_id}'";
                            int result = dbc.UpdateData(sql);
                            if (result == 1)
                            {
                                context.Response.Headers["Content-Type"] = "application/json";
                                context.Response.StatusCode = StatusCodes.Status200OK;
                            }
                            else
                            {
                                Console.WriteLine("DB Update Error");
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            }
                        }
                    });
                    #endregion
                    
                    endpoints.MapPost("/getData", async context =>
                    {
                        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                        {
                            var sql = "";
                            var answer = new JsonObject();
                            JsonDocument requestBody = await JsonDocument.ParseAsync(context.Request.Body);
                            var token = requestBody.RootElement.GetProperty("token").GetString();
                            var user_id = dbc.FindId(token);
                            sql = $"select * from skill where id = '{user_id}'";
                            dbc.SearchTable(sql, "skill", answer);
                            answer["token"] = token;
                            Console.WriteLine(answer.ToString());
                            context.Response.Headers["Content-Type"] = "application/json";
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            await context.Response.WriteAsync(answer.ToString());
                        }
                    });
                });
            }
            catch (Exception e)
            {
                
            }
        }
        
        // private Task HandlePostRequest(HttpContext context)
        // {
        //     if (context.Request.Method == HttpMethods.Post)
        //     {
        //         string requestBody;
        //         using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        //         {
        //             requestBody = reader.ReadToEnd();
        //         }
        //
        //         // TODO: 요청 데이터 처리 로직 작성
        //
        //         context.Response.StatusCode = StatusCodes.Status200OK;
        //         return context.Response.WriteAsync("POST 요청이 성공적으로 처리되었습니다.");
        //     }
        //
        //     context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        //     return Task.CompletedTask;
        // }
    }
    


    public class Program
    {
        public static void Main(string[] args)
        {
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