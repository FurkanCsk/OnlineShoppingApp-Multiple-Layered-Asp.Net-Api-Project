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
        private readonly IUserService _userService; // Service for user-related operations

        public AuthController(IUserService userService)
        {
            _userService = userService; // Injecting the user service through dependency injection
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // Validate the request model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a DTO for adding a user
            var addUserDto = new AddUserDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
            };

            // Call the user service to add a new user
            var result = await _userService.AddUser(addUserDto);

            if(result.IsSucceed)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message); // Return failure message if user registration fails
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a DTO for user login
            var loginUserDto = new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password
            };

            // Attempt to log in the user
            var result = _userService.LoginUser(new LoginUserDto { Email = request.Email, Password = request.Password});

            if(!result.IsSucceed)
            {
                return BadRequest(result.Message); // Return failure message if login fails
            }

            var user = result.Data;

            // Retrieve JWT configuration from the request services
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            // Generate a JWT token for the user
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

            // Return the success response with the token
            return Ok(new LoginResponse
            {
                Message = "Login successful",
                Token = token,
            });
        }

        [HttpGet("me")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> GetMyUser()
        {
            return Ok(); // Return user information (to be implemented)
        }
    }
}
