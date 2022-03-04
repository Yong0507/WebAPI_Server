using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using CloudStructures.Structures;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MySqlConnector;
using WebAPI_Server.DB;

namespace WebAPI_Server.Controllers
{
    [ApiController]
    [Route("CreateAccount")]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        public async Task<CreateAccountPacketRes> CreateAccount(CreateAccountPacketReq req)
        {
            CreateAccountPacketRes response = new CreateAccountPacketRes() {Result = ErrorCode.NONE};

            string salt = CryptographyProcessor.CreateSalt(20);
            string hashingPW = CryptographyProcessor.GenerateHash(req.userPW, salt);
                
            using (var connection = await DBManager.GetGameDBConnection())
            {
                try
                {
                    // 계정 기본 정보 INSERT
                     await connection.ExecuteAsync(
                        @"INSERT Users(userID, salt, hashingPW) Values(@userID, @salt, @hashingPW)",
                        new
                        {
                            userID = req.userID,
                            salt = salt,
                            hashingPW = hashingPW,
                        });
                    
                    // 우편함 기본 정보 INSERT
                    await connection.ExecuteAsync(
                        @"INSERT MailBox(MailBoxID, senderID, kind, Item, ItemCount) Values(@MailBoxID, @senderID, @kind, @Item, @ItemCount)",
                        new
                        {
                            MailBoxID = req.userID,
                            senderID = "Admin",
                            kind = KIND.GIFT,
                            Item = "Welcome Item",
                            ItemCount = 1
                        });
                }
                catch (Exception ex)
                {
                    // 중복 키 or 문자열 초과 문제 발생하면 곧바로 catch문으로 오게 됨
                    // --> 콘솔에서 Error 발생 원인 알려줌
                    
                    Console.WriteLine(ex.ToString());
                    response.Result = ErrorCode.Create_Account_Fail_Exception;
                    return response;
                }
            }
            return response;
        }
    }
}