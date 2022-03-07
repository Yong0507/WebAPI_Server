namespace WebAPI_Server
{
    public enum ErrorCode
    {
        NONE = 0,
        
        Create_Account_Fail_Exception = 11,

        Login_Fail_WrongPW = 12,
        Login_Fail_Exception = 13,
        
        MailBox_GetItem_Fail_Insert_Update = 14,
        MailBox_GetItem_Fail_Exception = 15,
        
        MailBox_Check_Fail_Exception = 16,
        
        Users_Ranking_Fail_Nothing_Data = 17,

        My_Ranking_Fail_Exception = 18,
        
        JwtToekn_Fail_Auth = 99,
    }
}

