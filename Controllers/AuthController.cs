using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlogAPI.Models;
using BlogAPI.Models.Dto;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
   // private User user = new User();
    private readonly IConfiguration _configuration;
    private AppDBContext _context;

    public AuthController(IConfiguration configuration, AppDBContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpGet]
    [Authorize]
    [Route("/me")]
    public async Task<ActionResult<User>> me()
    {
        var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        return Ok(user);
    }

    [HttpPost]
    [Route("/register")]
    public async Task<ActionResult<User>> register(UserDTO request)
    {
        var user = new User();
        var passwordData = createPasswordHash(request.password);
        user.username = request.username;
        user.passwordHash = passwordData["passwordHash"];
        user.passwordSalt = passwordData["passwordSalt"];
       await _context.Users.AddAsync(user);
       await _context.SaveChangesAsync();
       return Ok(user);
    }

    [HttpPost]
    [Route("/login")]
    public ActionResult login(UserDTO request)
    {
        var user = _context.Users.FirstOrDefault(u => u.username == request.username);

        if (user == null) return BadRequest("User doesn't exist");
        
       // if (user.username != request.username) return BadRequest();

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
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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