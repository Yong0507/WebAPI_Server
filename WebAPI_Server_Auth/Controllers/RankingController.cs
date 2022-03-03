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
    [Route("Ranking")]
    public class RankingController : ControllerBase
    {
        [HttpPost]
        public async Task<EndingPacketRes> CheckMailBox(EndingPacketReq req)
        {
            EndingPacketRes response = new EndingPacketRes();

            bool isValidate = JwtTokenProcessor.ValidateJwtAccessToken(req.JwtAccessToken, "kimyongjin123456789");
            
            if (!isValidate) {
                Console.WriteLine("Failed To Validate (invalid Token)");
                return response;
            }
            
            else {
                var DefaultExpiry = TimeSpan.FromDays(1);
                var set = new RedisSortedSet<string>(DBManager.RedisConn, "ranking", DefaultExpiry);
                await set.AddAsync(req.userID, req.score);
                var result = await set.RangeByScoreAsync();

                List<string> list = new List<string>();

                list = result.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    //Console.WriteLine("list : {0}" + Environment.NewLine, list[i]);
                    response.RankingList += list[i] + "##";
                }
                return response;
            }
        }

    }
}