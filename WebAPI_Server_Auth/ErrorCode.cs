namespace WebAPI_Server
{
    public enum ErrorCode
    {
        NONE = 0,
        
        Create_Account_Fail_Duplicate = 11,
        Create_Account_Fail_Exception = 12,
        Create_Mailbox_Fail_Duplicate = 13,

        Login_Fail_WrongPW = 14,
        Login_Fail_Exception = 15,
        
        MailBox_GetItem_Fail_Exception = 16,
        
        JwtToekn_Fail_Auth = 99,
    }
}

