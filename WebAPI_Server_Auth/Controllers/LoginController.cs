using System;
using System.Threading.Tasks;
using CloudStructures.Structures;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json.Serialization;
using WebAPI_Server.DB;

namespace WebAPI_Server.Controllers
{
    [ApiController]
    [Route("Login")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<LoginPacketRes> Login(LoginPacketReq req)
        {
            LoginPacketRes response = new LoginPacketRes() {Result = ErrorCode.NONE};
            
            using (var connection = await DBManager.GetGameDBConnection())
            {
                try
                {
                    var userInfo = await connection.QuerySingleOrDefaultAsync<DBUserInfo>(
                        "select userID, salt, hashingPW from Users where userID = @userID",
                        new
                        {
                            userID = req.userID
                        });
                    
                    bool verified = CryptographyProcessor.AreEqual(req.userPW, userInfo.hashingPW, userInfo.Salt);

                    if (verified == true)
                    {
                        response.JwtAccessToken = JwtTokenProcessor.CreateJwtAccessToken(userInfo.userID);
                        return response;
                    }
                
                    else
                    {
                        response.Result = ErrorCode.Login_Fail_WrongPW;
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    response.Result = ErrorCode.Login_Fail_Exception;
                    return response;
                }
            }
        }
    }
}