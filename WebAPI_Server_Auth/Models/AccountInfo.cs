namespace WebAPI_Server.Models
{
    public class AccountInfo
    {
        public string userID { get; set; }
        public string salt { get; set; }
        public string hashingPW { get; set; }
    }
}