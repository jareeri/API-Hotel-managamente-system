using Hotel.Validation;

namespace Hotel.DTO
{
    public class SingUpDTO 
    {
        public string? Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        //[DOBValidation(ErrorMessage = "invalid date of birth.")]
        //public DateTime DOB { get; set; }
        public string? Password { get; set; }

        public string RoleName { get; set; }
    }
}
