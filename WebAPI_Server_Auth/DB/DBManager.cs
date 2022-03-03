using System.Threading.Tasks;
using CloudStructures;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace WebAPI_Server.DB
{
    public class DBManager
    {
        static string MysqlDBString;
        static string RedisDBString;

        public static RedisConnection RedisConn { get; set; }
        
        
        public static void Init(IConfiguration configuration)
        {
            MysqlDBString = configuration.GetSection("DBConnection")["Mysql"];
            RedisDBString = configuration.GetSection("DBConnection")["Redis"];
            
            var config = new RedisConfig("basic", RedisDBString);
            RedisConn = new RedisConnection(config);
        }

        
        public static async Task<MySqlConnection> GetGameDBConnection()
        {
            return await GetOpenMySqlConnection(MysqlDBString);
        }
        
        static async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
        {
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
        
        
        
    }
}