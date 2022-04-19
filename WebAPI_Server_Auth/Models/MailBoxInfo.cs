namespace WebAPI_Server.Models
{
    public class MailBoxInfo
    {
        public string MailBoxID { get; set; }
        public string senderID { get; set; }
        public int kind { get; set; }
        public string Item { get; set; }
        public int ItemCount { get; set; }
    }
}