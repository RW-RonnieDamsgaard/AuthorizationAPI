using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationAPILibrary.Models;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static Dictionary<int, User> Users = new Dictionary<int, User>
        {
            { 1, new User { Id = 1, Username = "testuser", PasswordHash = HashPassword("password"), Role = "Subscriber" } },
            { 2, new User { Id = 2, Username = "writeruser", PasswordHash = HashPassword("password"), Role = "Writer" } },
            { 3, new User { Id = 3, Username = "writer2user", PasswordHash = HashPassword("password"), Role = "Writer" } },
            { 4, new User { Id = 4, Username = "editoruser", PasswordHash = HashPassword("password"), Role = "Editor" } }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = Users.Values.FirstOrDefault(u => u.Username == login.Username);
            if (user != null && VerifyPassword(login.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Ensure user ID is a string representation of an integer
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    
}
