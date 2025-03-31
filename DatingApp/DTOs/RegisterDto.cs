using System.ComponentModel.DataAnnotations;

namespace DatingApp.DTOs
{
    public class RegisterDto
    {
      
        public string? Username { get; set; }=string.Empty;
        public string? KnownAs { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PasswordRegister { get; set; }= string.Empty;
    }
}
