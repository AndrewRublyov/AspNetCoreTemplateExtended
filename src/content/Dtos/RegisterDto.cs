using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTemplateExtended.Dtos
{
  public partial class AccountController
  {
    public class RegisterDto
    {
      [Required]
      public string Email { get; set; }

      [Required]
      [StringLength(100, ErrorMessage = "Password length must be between 6 and 100 symbols", MinimumLength = 6)]
      public string Password { get; set; }
    }
  }
}