using System.ComponentModel.DataAnnotations;
#nullable disable
namespace TTT.Models
{
    public class PlayerDTO
    {
        [Required(ErrorMessage = "Слишком короткое имя")]
        [StringLength(100, ErrorMessage = "Слишком короткое имя", MinimumLength = 2)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Пароль должен быть длиной от 6 символов")]
        [StringLength(100, ErrorMessage = "Пароль должен быть длиной от 6 символов", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
