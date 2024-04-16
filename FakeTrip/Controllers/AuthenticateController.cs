using FakeTrip.Constants;
using FakeTrip.Dtos;
using FakeTrip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FakeTrip.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IConfiguration config;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthenticateController(
        IConfiguration config,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.config = config;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        var loginResult = await ValidateCrendentials(loginDto);
        if (loginResult is null || !loginResult.Succeeded)
        {
            return BadRequest();
        }
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        string token = await GenerateTokenAsync(user!);
        return Ok(token);
    }

    private async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(config.GetValue<string>(AuthenticationConstant.SecretKey)!)
            );

        var signingCrendentials = new SigningCredentials(secretKey,
            SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [];
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id));

        var roleNames = await userManager.GetRolesAsync(user);
        foreach (var role in roleNames)
        {
            var roleClaim = new Claim(ClaimTypes.Role, role);
            claims.Add(roleClaim);
        }

        var token = new JwtSecurityToken(
            config.GetValue<string>(AuthenticationConstant.Issuer),
            config.GetValue<string>(AuthenticationConstant.Audience),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(AuthenticationConstant.ExpDay),
            signingCrendentials
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<Microsoft.AspNetCore.Identity.SignInResult> ValidateCrendentials(LoginDto loginDto)
    {
        var res = await signInManager.PasswordSignInAsync(
            loginDto.Email,
            loginDto.Password,
            false,
            false);
        return res;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new ApplicationUser()
        {
            UserName = registerDto.Email,
            Email = registerDto.Email
        };
        
        var res = await userManager.CreateAsync(user, registerDto.Password);
        if (!res.Succeeded)
        {
            return BadRequest(res.Errors.FirstOrDefault()?.Description);
        }
        return Ok();
    }
}
