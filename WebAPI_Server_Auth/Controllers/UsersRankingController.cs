using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Server;
using WebAPI_Server.DB;

namespace WebAPI_Server_Auth.Controllers
{
    [ApiController]
    [Route("UsersRanking")]
    public class UsersRankingController : ControllerBase
    {
        [HttpPost]
        public async Task<UsersRankingPacketRes> UsersRanking()
        {
            UsersRankingPacketRes response = new UsersRankingPacketRes() {Result = ErrorCode.NONE};

            var RedisData = new RedisSortedSet<string>(DBManager.RedisConn, "Ranking", null);
            var UsersRanking = await RedisData.RangeByScoreAsync(order: StackExchange.Redis.Order.Descending);

            if (UsersRanking.Length < 1)
            {
                response.Result = ErrorCode.Users_Ranking_Fail_Nothing_Data;
                return response;
            }

            List<string> Rankinglist = new List<string>();
            Rankinglist = UsersRanking.ToList();
            
            for (int i = 0; i < Rankinglist.Count; i++)
            {
                response.RankingList += $"{Rankinglist[i]} : {i + 1}등 # ";
            }

            return response;
        }
    }
}