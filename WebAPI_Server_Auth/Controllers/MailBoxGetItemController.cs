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
                    var UpdateItem = await connection.ExecuteAsync(
                        @"UPDATE MailBox set ItemCount = ItemCount + 1 where Item = @Item AND MailBoxID = @MailBoxID",
                        new
                        {
                            MailBoxID = userID,
                            Item = req.Item,
                        });

                    // UPDATE 실패(0을 리턴) --> 아이템 존재하지 않음 --> 이때 INSERT 
                    // ON DUPLICATE KEY UPDATE 활용하려했지만 --> Item의 값을 PK로 두는 것은 아니라고 생각했음(Unique하지 않고 NULL 일수도 있기 때문)
                    
                    // 아이템 정보 INSERT
                    if (UpdateItem != 1)
                    {
                        var InsertItem = await connection.ExecuteAsync(
                            @"INSERT INTO MailBox(MailBoxID, senderID, kind, Item, ItemCount) SELECT @MailBoxID, @senderID, @kind, @Item, @ItemCount FROM DUAL " +
                                 "WHERE EXISTS(select * from MailBox where MailBoxID = @MailBoxID)",
                            new
                            {
                                MailBoxID = userID,
                                senderID = "Admin",
                                kind = KIND.GIFT,
                                Item = req.Item,
                                ItemCount = 1
                            });
                        
                        if (InsertItem != 1)
                        {
                            response.Result = ErrorCode.MailBox_GetItem_Fail_Insert;
                            return response;
                        }
                    }
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