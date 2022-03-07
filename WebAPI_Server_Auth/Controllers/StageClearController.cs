using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudStructures.Structures;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebAPI_Server;
using WebAPI_Server.DB;

namespace WebAPI_Server_Auth.Controllers
{
    [ApiController]
    [Route("StageClear")]
    public class StageClearController : ControllerBase
    {
        [HttpPost]
        public async Task<StageClearPacketRes> CheckMailBox(StageClearPacketReq req)
        {
            StageClearPacketRes response = new StageClearPacketRes() {Result = ErrorCode.NONE};

            bool isValidate = JwtTokenProcessor.ValidateJwtAccessToken(req.JwtAccessToken, JwtTokenProcessor.UniqueKey);
            
            if (isValidate == false) 
            {
                response.Result = ErrorCode.JwtToekn_Fail_Auth;
                return response;
            }

            if (req.Score < 0)
            {
                response.Result = ErrorCode.Stage_Clear_Fail_Negative_Score;
                return response;
            }

            var token = JwtTokenProcessor.DecipherJwtAccessToken(req.JwtAccessToken);
            string userID = token.Subject;
            
            var DefaultExpiry = TimeSpan.FromDays(1);
            var set = new RedisSortedSet<string>(DBManager.RedisConn, "Ranking", DefaultExpiry);
            await set.AddAsync(userID, req.Score);
            
            return response;
        }
    }
}