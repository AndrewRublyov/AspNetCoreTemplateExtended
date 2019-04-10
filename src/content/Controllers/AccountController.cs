using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTemplateExtended.Config.Options;
using AspNetCoreTemplateExtended.Data.Entities;
using AspNetCoreTemplateExtended.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreTemplateExtended.Controllers
{
  // TODO: Move logic to service
  [ApiController]
  [Route("account")]
  public class AccountController : ControllerBase
  {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IOptions<AuthOptions> _options;

    public AccountController(
      UserManager<User> userManager,
      SignInManager<User> signInManager,
      IOptions<AuthOptions> options
    )
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _options = options;
    }

    [HttpPost("authorize")]
    public async Task<object> Authorize([FromBody] Dtos.AccountController.AuthorizeDto model)
    {
      var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

      if (result.Succeeded)
      {
        var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
        var token = GenerateJwtToken(model.Email, appUser);
        return new {token};
      }

      throw new BusinessLogicException($"Failed to authorize user, reason: {result}");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Dtos.AccountController.RegisterDto model)
    {
      var user = new User
      {
        UserName = model.Email,
        Email = model.Email
      };
      var result = await _userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        await _signInManager.SignInAsync(user, false);
      }

      return Ok();
    }

    private string GenerateJwtToken(string email, IdentityUser user)
    {
      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Key));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expires = DateTime.Now.AddDays(Convert.ToDouble(_options.Value.ExpireDays));

      var token = new JwtSecurityToken(
        _options.Value.Issuer,
        _options.Value.Issuer,
        claims,
        expires: expires,
        signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}