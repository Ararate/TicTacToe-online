using Microsoft.EntityFrameworkCore;
using TTT.Models;

namespace TTT.Data
{
    public class DataBase : DbContext
    {
        public DbSet<GameResult> GameResults { get; set; }
        public DbSet<Player> Players { get; set; }
        public DataBase(DbContextOptions options) : base(options)
        {

        }
    }
}
