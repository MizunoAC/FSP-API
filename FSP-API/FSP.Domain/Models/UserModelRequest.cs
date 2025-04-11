using System.ComponentModel.DataAnnotations;

namespace FSP.Domain.Models
{
    public class UserModelRequest
    {
        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        public string Locality { get; set; }

        [Required]
        public int Gender { get; set; }

        public int Age { get; set; }

        [Required
        ,StringLength(50), 
         EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public string UserType { get; set; } = string.Empty;
    }
}