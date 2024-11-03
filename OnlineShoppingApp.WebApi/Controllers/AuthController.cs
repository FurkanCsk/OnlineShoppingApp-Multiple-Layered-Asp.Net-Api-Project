using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingApp.Business.Operations.User;
using OnlineShoppingApp.Business.Operations.User.Dtos;
using OnlineShoppingApp.WebApi.Jwt;
using OnlineShoppingApp.WebApi.Models;

namespace OnlineShoppingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } // TODO: Will code with action filter


            var addUserDto = new AddUserDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userService.AddUser(addUserDto);

            if(result.IsSucceed)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState); // ACTION FILTER UGULANACAK
            }

            var loginUserDto = new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = _userService.LoginUser(new LoginUserDto { Email = request.Email, Password = request.Password});

            if(!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }

            var user = result.Data;

            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var token = JwtHelper.GenerateJwtToken(new JwtDto
            {
                Id = user.Id,
                Email =  user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                SecretKey = configuration["Jwt:SecretKey"]!,
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"]!)
            });

            return Ok(new LoginResponse
            {
                Message = "Login successful",
                Token = token,
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyUser()
        {
            return Ok();
        }
    }
}
