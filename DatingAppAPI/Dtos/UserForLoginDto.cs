using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.Dtos
{
  public class UserForLoginDto
  {
    [Required]
    public string Username { get; set; }

    [Required]
    [StringLength(8, MinimumLength = 4, ErrorMessage = "Ypo must specify password between 4 and 8 characters")]
    public string Password { get; set; }
  }
}