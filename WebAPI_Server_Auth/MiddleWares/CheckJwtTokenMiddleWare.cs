using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAPI_Server.DB;

namespace WebAPI_Server.MiddleWares
{
    public class CheckJwtTokenMiddleWare
    {
        private readonly RequestDelegate _next;
        
        public CheckJwtTokenMiddleWare(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
        }

        // 로그인 이후 API 호출마다 JwtToken 검증
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != "/CreateAccount" &&
                context.Request.Path != "/Login")
            {
                StreamReader bodystream = new StreamReader(context.Request.Body, Encoding.UTF8);
                string body = bodystream.ReadToEndAsync().Result;
                var obj = (JObject) JsonConvert.DeserializeObject(body);

                var userJwtToken = (string) obj["jwtAccessToken"];
                
                bool isValidate = JwtTokenProcessor.ValidateJwtAccessToken(userJwtToken, JwtTokenProcessor.UniqueKey);
                
                if (isValidate == false)
                {
                    var response = new ResBase() {Result = ErrorCode.JwtToekn_Fail_Auth};
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    return;
                }
                
                //위에 Body를 읽을 때 위치가 이동해서 다시 덮어쓴다.
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            }

            await _next(context);
        }


    }
}