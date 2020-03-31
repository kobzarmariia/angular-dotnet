using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Dtos;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingAppAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _repository;

    public readonly IConfiguration _configuration;

    public AuthController(IAuthRepository repository, IConfiguration configuration)
    {
      _repository = repository;
      _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
    {
      // validate request if it isnt provided [ApiController]
      // if (!ModelState.IsValid)
      //   return BadRequest(ModelState);

      userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

      if (await _repository.UserExists(userForRegisterDto.Username))
        return BadRequest("Username already exists");

      var userToCreate = new User
      {
        Username = userForRegisterDto.Username
      };

      var createdUser = await _repository.Register(userToCreate, userForRegisterDto.Password);

      return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
    {
      var userFromRepository = await _repository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

      if (userFromRepository == null)
        return Unauthorized();

      var claims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, userFromRepository.Id.ToString()),
        new Claim(ClaimTypes.Name, userFromRepository.Username)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)); //from appsettings.json

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return Ok(new
      {
        token = tokenHandler.WriteToken(token)
      });
    }
  }
}