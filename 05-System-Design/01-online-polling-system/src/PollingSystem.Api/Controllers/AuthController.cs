using Microsoft.AspNetCore.Mvc;
using PollingSystem.Api.Auth;
using PollingSystem.Domain.Enums;

namespace PollingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// JWT stub — در production از Identity Server یا OAuth2 استفاده شود.
    /// </summary>
    [HttpPost("token")]
    public IActionResult GetToken([FromBody] TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest("userId الزامی است.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return BadRequest("نقش معتبر: Admin, User, Analyst");

        var token = JwtTokenGenerator.GenerateToken(request.UserId, role);
        return Ok(new { access_token = token, token_type = "Bearer", role = role.ToString() });
    }
}

public record TokenRequest(string UserId, string Role);
