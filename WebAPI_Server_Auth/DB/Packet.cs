namespace WebAPI_Server.DB
{
    public enum KIND
    {
        NOTICE = 1,
        PATCH_NOTE,
        GIFT,
        LEVELUP,
        ATTENDANCE
    }
    
    // --------------------------- //
    // ----------DataBase--------- //
    // --------------------------- //

    public class DBUserInfo
    {
        public string userID;
        public string Salt;
        public string hashingPW;
    }

    public class DBMailBoxInfo
    {
        public string MailBoxID;
        public string senderID;
        public int kind;
        public string Item;
        public int ItemCount;
    }

    public class DBItemInfo
    {
        public string userID;
        public string Item;
        public int ItemCount;
    }

    // --------------------------- //
    // ----------Packet----------- //
    // --------------------------- //
    
    public class CreateAccountPacketReq
    {
        public string userID { get; set; }
        public string userPW { get; set; }
    }

    public class CreateAccountPacketRes 
    {
        public ErrorCode Result { get; set; }
    }
    
    public class LoginPacketReq
    {
        public string userID { get; set; }
        public string userPW { get; set; }
    }

    public class LoginPacketRes
    {
        public ErrorCode Result { get; set; }
        public string JwtAccessToken { get; set; }
    }
    
    public class MailBoxCheckPacketReq
    {
        public string JwtAccessToken { get; set; }
    }

    public class MailBoxCheckPacketRes
    {
        public ErrorCode Result { get; set; }
        public string MailBoxID { get; set; }
        public string senderID { get; set; }
        public int kind { get; set; }
        public string Item { get; set; }
        public int ItemCount { get; set; }
    }
    
    public class MailBoxItemPacketReq
    {
        public string JwtAccessToken { get; set; }
        public string Item { get; set; }        
    }
    
    public class MailBoxItemPacketRes
    {
        public ErrorCode Result { get; set; }
    }

    public class EndingPacketReq
    {
        public string userID { get; set; }
        public string JwtAccessToken { get; set; }
        public int score { get; set; }
    }

    public class EndingPacketRes
    { 
        public string RankingList { get; set; }
    }



}