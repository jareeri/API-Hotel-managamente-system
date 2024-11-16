using System.ComponentModel.DataAnnotations;

namespace Hotel.DTO
{
    public class SingInDTO
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
