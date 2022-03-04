using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Server;
using WebAPI_Server.DB;

namespace WebAPI_Server_Auth.Controllers
{
    [ApiController]
    [Route("MyRanking")]
    public class MyRankingController : ControllerBase
    {
        [HttpPost]
        public async Task<MyRankingPacketRes> MyRanking(MyRankingPacketReq req)
        {
            MyRankingPacketRes response = new MyRankingPacketRes() {Result = ErrorCode.NONE};

            bool isValidate = JwtTokenProcessor.ValidateJwtAccessToken(req.JwtAccessToken, JwtTokenProcessor.UniqueKey);

            if (isValidate == false)
            {
                response.Result = ErrorCode.JwtToekn_Fail_Auth;
                return response;
            }

            var token = JwtTokenProcessor.DecipherJwtAccessToken(req.JwtAccessToken);
            string userID = token.Subject;
            
            var RedisData = new RedisSortedSet<string>(DBManager.RedisConn, "Ranking", null);
            var MyRanking = await RedisData.RankAsync(userID, order: StackExchange.Redis.Order.Descending) ?? -1; // 반환값이 Nullable 타입, 랭킹인데 0부터 시작 --> 따라서 null+1을 허용안함 --> NULL일 때 처리 필요
            
            if (MyRanking == -1)
            {
                response.Result = ErrorCode.My_Ranking_Fail_Exception;
                return response;
            }
            
            response.MyRanking = (MyRanking + 1);

            var NearRanking = await RedisData.RangeByRankAsync(MyRanking - 1, MyRanking + 1,
                 order: StackExchange.Redis.Order.Descending);
            
            List<string> NearRankingList = new List<string>();
            NearRankingList = NearRanking.ToList();
            
            for (int i = 0; i < NearRankingList.Count; i++)
            {
                response.NearRanking += $"{NearRankingList[i]} : {MyRanking + i}등 # ";
            }
            
            
            return response;
        }
    }
}