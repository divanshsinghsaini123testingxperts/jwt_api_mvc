using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using webApi.Interface;
using webApi.Models.Data;
using webApi.Models.Entity;
using WebAPIDemo.Services;
namespace webApi.Services
{

    [Route("api/[controller]")]
    [ApiController] //used for auto model validation , and it will autometically throw 404 bad request error,automodel binding , error handling 
    public class AuthService : ControllerBase  , IAuthservice // Inherit from ControllerBase to use BadRequest and other helper methods
    {
        private readonly AppDbcontext _context;
        private readonly TokenService _tokenService;

        public AuthService(AppDbcontext context, TokenService tokenwali_service)
        {
            _tokenService = tokenwali_service;
            _context = context;
        }

        [HttpPost("Register")]
        public IActionResult Register(User user)
        {
            user.Id = 0; // Ensure the ID is not explicitly set to avoid conflict with identity column

            var eml = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (eml != null)
            {
                return BadRequest("Email already exists."); // Fixed: BadRequest is now accessible via ControllerBase
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user); // Return 200 with the registered user data
        }

        [HttpPost("LoginRequest")]
        public IActionResult LoginRequest(Login_details login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (user != null)
            {
                var token = _tokenService.GenerateToken(login.Email);
                return Ok(new {token = token });
            }
            return Unauthorized();
        }
    }
}
