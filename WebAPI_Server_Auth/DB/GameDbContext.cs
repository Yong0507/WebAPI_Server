using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI_Server.Models;

namespace WebAPI_Server.DB
{ 
    public class GameDbContext : DbContext
    {
        protected GameDbContext(DbContextOptions options) : base(options)
        {
        }

        public void StartBulkUpdate()
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public async Task EndBulkUpdate()
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
            await SaveChangesAsync();
        }
        
        public virtual DbSet<AccountInfo> AccountInfos { get; set; }
        
        public virtual DbSet<MailBoxInfo> MailBoxInfos { get; set; }
    }
}