using System;
using System.ComponentModel.DataAnnotations;
using API.Helpers;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [RegularExpression("(male)|(female)", ErrorMessage = "The Gender must be either 'male' or 'female' only.")]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        [MinAge(18)]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}