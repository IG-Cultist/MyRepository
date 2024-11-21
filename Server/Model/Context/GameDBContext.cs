using Microsoft.EntityFrameworkCore;
using Shared.Model.Entity;

namespace Server.Model.Context
{
    public class GameDBContext:DbContext
    {
        // テーブル(Entity)を追加したらここに記入
        public DbSet<User> Users { get; set; }

        readonly string connectionString =
            "server=localhost;database=realtime_game;user=jobi;password=jobi;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString , new MySqlServerVersion(new Version(8, 0)));
        }
    }
}
