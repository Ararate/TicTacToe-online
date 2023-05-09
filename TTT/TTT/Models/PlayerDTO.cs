using System.ComponentModel.DataAnnotations;

namespace TTT.Models
{
    public class PlayerDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Слишком короткое имя", MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен быть длиной от 6 символов", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
