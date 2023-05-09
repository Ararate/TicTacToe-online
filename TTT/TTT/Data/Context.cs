using Microsoft.EntityFrameworkCore;
using TTT.Models;

namespace TTT.Data
{
    public class Context : DbContext
    {
        public DbSet<GameResult> GameResults { get; set; }
        public DbSet<Player> Players { get; set; }
        public Context(DbContextOptions options) : base(options)
        {

        }
    }
}
