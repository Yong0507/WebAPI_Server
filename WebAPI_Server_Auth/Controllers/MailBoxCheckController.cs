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
        public async Task<List<MailBoxCheckPacketRes>> CheckMailBox(MailBoxCheckPacketReq req)
        {
            List<MailBoxCheckPacketRes> response = new List<MailBoxCheckPacketRes>();
            response.Add(new MailBoxCheckPacketRes() {Result = ErrorCode.NONE});
            
            var token = JwtTokenProcessor.DecipherJwtAccessToken(req.JwtAccessToken);
            string userID = token.Subject;
            
            using (MySqlConnection connection = await DBManager.GetGameDBConnection())
            {
                try
                {
                    var mailInfo = await connection.QueryAsync<MailBoxCheckPacketRes>(
                        "select MailBoxID, senderID, kind, Item, ItemCount from MailBox where MailBoxID = @mailBoxID",
                        new
                        {
                            MailBoxID = userID
                        });

                    response.Clear();
                    response.AddRange(mailInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    response[0].Result = ErrorCode.MailBox_Check_Fail_Exception;
                    return response;
                }

                return response;
            }
        }
    }
}