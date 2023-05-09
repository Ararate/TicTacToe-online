using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TTT.Models
{
    public class GameResult
    {
        public int Id { get; set; }
        [Required]
        public string? HostName { get; set; }

        [ForeignKey("HostName")]
        public Player? Host { get; set; }
        [Required]
        public string? GuestName { get; set; }

        [ForeignKey("GuestName")]
        public Player? Guest { get; set; }
        public string? Winner { get; set; }
        public bool Draw { get; set; }
    }
}
