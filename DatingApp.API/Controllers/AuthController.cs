using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _AuthRepository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository AuthRepository, IConfiguration config)
        {
            _config = config;
            _AuthRepository = AuthRepository;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(201);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userdto)
        {
            userdto.username = userdto.username.ToLower();

            if (await _AuthRepository.UserExists(userdto.username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userdto.username
            };

            var createdUser = await _AuthRepository.Register(userToCreate, userdto.password);

            return StatusCode(201);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userdto)
        {
            var userFromRepo = await _AuthRepository.Login(userdto.username.ToLower(), userdto.password);
            if (userFromRepo == null) return Unauthorized();

            // create claims
            var claims = new[]
            {
           new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
           new Claim(ClaimTypes.Name,userFromRepo.Username.ToString())
            };

        // the key of jwt
        var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));
 
        // that take key and algorithem
         var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

        // make jwt 1- claims 2- expiredate 3- SigningCredentials
         var tokenDescriptor = new SecurityTokenDescriptor
         {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
         };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new {
             token = tokenHandler.WriteToken(token)
        });
       }
    }
}