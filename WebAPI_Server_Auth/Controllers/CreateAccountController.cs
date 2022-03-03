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
                            kind = KIND.NOTICE,
                            Item = "Welcome Item",
                            ItemCount = 1
                        });
                }
                catch (Exception ex)
                {
                    // 기본적으로 Primary Key를 설정했기에 중복 키 문제 발생해도 catch문으로 오게 됨
                    // 중복 키 or 문자열 초과 등에 대한 문제 발생을 따로 구분은 하지 않음
                    // 출력값으로 어떠한 Error인지 알려줌
                    
                    Console.WriteLine(ex.ToString());
                    response.Result = ErrorCode.Create_Account_Fail_Exception;
                    return response;
                }
            }
            return response;
        }
    }
}