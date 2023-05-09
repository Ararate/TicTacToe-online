using System.ComponentModel.DataAnnotations;
using System.Text;
using TTT.Utility;
#nullable disable
namespace TTT.Models
{
    public class Player
    {
        public Player()
        {}
        public Player(PlayerDTO dto)
        {
            Name = dto.Name;
            Password = dto.Password;
        }
        [Key]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
