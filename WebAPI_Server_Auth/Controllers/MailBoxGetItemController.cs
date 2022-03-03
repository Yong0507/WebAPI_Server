using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebAPI_Server;
using WebAPI_Server.DB;

namespace WebAPI_Server_Auth.Controllers
{
    [ApiController]
    [Route("MailBoxGetItem")]
    public class MailBoxGetItemController : ControllerBase
    {
        [HttpPost]
        public async Task<MailBoxItemPacketRes> GetItemMailBox(MailBoxItemPacketReq req)
        {
            MailBoxItemPacketRes response = new MailBoxItemPacketRes() {Result = ErrorCode.NONE};
            
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
                try
                {
                    // 아이템 정보 UPDATE
                    var count = await connection.ExecuteAsync(
                        @"UPDATE MailBox set ItemCount = ItemCount + 1 where Item = @Item AND MailBoxID = @MailBoxID",
                        new
                        {
                            MailBoxID = userID,
                            Item = req.Item,
                        });

                    Console.WriteLine("UPDATE Item 리턴 값 : " + count);
                    // 0이면 UPDATE 실패니까 이때 INSERT 하면 된다 !!!! 오케이
                    
                    // 아이템 정보 INSERT
                    var count1 = await connection.ExecuteAsync(
                        @"INSERT MailBox(MailBoxID, senderID, kind, Item, ItemCount) Values(@MailBoxID, @senderID, @kind, @Item, @ItemCount)",
                        new
                        {
                            MailBoxID = userID,
                            senderID = "Admin",
                            kind = KIND.NOTICE,
                            Item = "Welcome Item3",
                            ItemCount = 1
                        });


                    
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    response.Result = ErrorCode.MailBox_GetItem_Fail_Exception;
                    return response;
                }
                
                return response;
            }
        }
        
    }
    
    
    
}