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

namespace WebAPI_Server.Controllers
{
    [ApiController]
    [Route("MailBoxCheck")]
    public class MailBoxController : ControllerBase
    {
        [HttpPost]
        public async Task<MailBoxCheckPacketRes> CheckMailBox(MailBoxCheckPacketReq req)
        {
            MailBoxCheckPacketRes response = new MailBoxCheckPacketRes() {Result = ErrorCode.NONE};
            
            bool isValidate = JwtTokenProcessor.ValidateJwtAccessToken(req.JwtAccessToken, JwtTokenProcessor.UniqueKey);

            if (isValidate == false)
            {
                response.Result = ErrorCode.JwtToekn_Fail_Auth;
                return response;
            }

            var token = JwtTokenProcessor.DecipherJwtAccessToken(req.JwtAccessToken);
            string userID = token.Subject;

            using (MySqlConnection connection = await DBManager.GetGameDBConnection())
            {
                var mailInfo = await connection.QuerySingleOrDefaultAsync<DBMailBoxInfo>("select MailBoxID, senderID, kind, Item, ItemCount from MailBox where MailBoxID = @mailBoxID",
                         new
                         {
                             MailBoxID = userID
                         });
                
                response.MailBoxID = mailInfo.MailBoxID;
                response.senderID = mailInfo.senderID;
                response.kind = mailInfo.kind;
                response.Item = mailInfo.Item;
                response.ItemCount = mailInfo.ItemCount;
                
                return response;
            }
        }
    }
}