using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PriconnePartyManager.Scripts.Sql.Model;

namespace PriconnePartyManager.Scripts.Sql
{
    public class SqlConnector : DbContext
    {
        public DbSet<UnitProfile> UnitProfiles { get; internal set; }
        public DbSet<UnitData> UnitData { get; internal set; }
        public DbSet<UnitPlayable> UnitPlayables { get; private set; }
        public DbSet<UnlockRarity6> UnlockRarity6 { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            var connectionString =
                new SqliteConnectionStringBuilder { DataSource = @"./data/redive_jp.db" }.ToString();
            optionBuilder.UseSqlite(new SqliteConnection(connectionString));
        }
    }
}