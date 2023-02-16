using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogAPI.Models;
using BlogAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    public static User user = new User();
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("/register")]
    public ActionResult<User> register(UserDTO request)
    {
        var passwordData = createPasswordHash(request.password);
        user.username = request.username;
        user.passwordHash = passwordData["passwordHash"];
        user.passwordSalt = passwordData["passwordSalt"];
        return Ok(user);
    }

    [HttpPost]
    [Route("/login")]
    public ActionResult login(UserDTO request)
    {
        if (user.username != request.username) return BadRequest();

        if (!verifyPasswordHash(request.password, user.passwordHash, user.passwordSalt))
        {
            return BadRequest("Wrong password");
        }

        var token = createToken(user);

        return Ok(token);
    }



    private string createToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.username)
        };

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: cred
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }


    private Dictionary<string, byte[]> createPasswordHash(string password)
    {
        Dictionary<string, byte[]> passwordData = new Dictionary<string, byte[]>();
        var hmac = new HMACSHA512();
        passwordData.Add("passwordSalt", hmac.Key);
        passwordData.Add("passwordHash", hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        return passwordData;
    }
    
}