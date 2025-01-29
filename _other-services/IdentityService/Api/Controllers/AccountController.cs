using IdentityService.Api.Contracts;
using IdentityService.Api.Services;
using IdentityService.Domain.AppUserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<AppUser> userManager, TokenService tokenService) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly TokenService _tokenService = tokenService;
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] MyRegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            UserName = request.Username,
            Email = request.Email
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(new
        {
            Message = "User created successfully."
        });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] MyLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        
        if (user is null)
        {
            return Unauthorized();
        }
        var signInResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!signInResult)
        {
            return Unauthorized();
        }
        
        var roles = await _userManager.GetRolesAsync(user);
    
        var token = await _tokenService.GenerateTokenAsync(user, roles);
        
        return Ok(token);
    }
}