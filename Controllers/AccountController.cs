using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankAccountManager.Data;
using BankAccountManager.Enums;
using BankAccountManager.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BankAccountManager.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(
    IConfiguration configuration,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager
) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    public async Task<IActionResult> Register([FromBody] AddOrUpdateUserModel user)
    {
        //check if user already exists
        var userExists = await _userManager.FindByEmailAsync(user.Email);
        if (userExists != null)
        {
            ModelState.AddModelError("Email", "User already exists");
            return BadRequest(ModelState);
        }

        //validate user fields
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //create user
        var newUser = new AppUser { UserName = user.Username, Email = user.Email };
        var result = await _userManager.CreateAsync(newUser, user.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        await _userManager.AddToRoleAsync(newUser, AppUserRoles.User.ToString());
        var token = GenerateToken(newUser);
        return Ok(new { token });
    }

    public async Task<string?> GenerateToken(AppUser currentUser)
    {
        var username = currentUser.UserName;
        if (string.IsNullOrEmpty(username))
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(currentUser);

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var secret = _configuration["Jwt:Secret"];

        if (
            string.IsNullOrEmpty(issuer)
            || string.IsNullOrEmpty(audience)
            || string.IsNullOrEmpty(secret)
        )
        {
            throw new ApplicationException("Invalid JWT issuer or audience");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, roles.FirstOrDefault() ?? AppUserRoles.User.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        return token;
    }

    public async Task<IActionResult> Login([FromBody] LoginModel user)
    {
        var userExists = await _userManager.FindByEmailAsync(user.Email);
        if (userExists == null)
        {
            ModelState.AddModelError("Email", "User does not exist");
            return BadRequest(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.Email,
            user.Password,
            false,
            false
        );

        if (!result.Succeeded)
        {
            ModelState.AddModelError("Password", "Invalid password");
            return BadRequest(ModelState);
        }

        var token = GenerateToken(userExists);
        return Ok(new { token });
    }
}
