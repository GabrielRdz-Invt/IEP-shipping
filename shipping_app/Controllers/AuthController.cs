using Microsoft.AspNetCore.Mvc;
using shipping_app.Security;

namespace shipping_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILdapAuthService _ldap;
        public AuthController(ILdapAuthService ldap) => _ldap = ldap;

        public class LoginDto
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("User is required");
            var (ok, identity, error) = await _ldap.ValidateAsync(dto.Username, dto.Password);
            if (!ok) return Unauthorized(new { message = error ?? "Invalid Credentials" });
            return Ok(new { user = identity });

        }
    }
}