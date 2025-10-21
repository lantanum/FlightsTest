using Flights.Application.Abstractions.Repositories;
using Flights.Application.Users.Dtos;
using Flights.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Flights.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly JwtTokenService _jwt;

        public AuthController(IUserRepository users, JwtTokenService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        /// <summary>
        /// Получить JWT-токен по логину и паролю.
        /// </summary>
        [HttpPost("token")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Token([FromBody] LoginRequest req, CancellationToken ct)
        {
            var user = await _users.GetWithRoleByUsernameAsync(req.Username, ct);
            if (user is null || !PasswordHasher.Verify(req.Password, user.Password))
                return Unauthorized();

            var token = _jwt.CreateToken(user.Username, user.Role.Code);
            return Ok(token);
        }
    }
}
