using Microsoft.AspNetCore.Authorization;
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
        //private int otp = 0;
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

        [HttpPost("Forgot_Password_mailcheck")]
        public IActionResult Forgot_Password_mailcheck(Emailh e)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == e.Email);
            if (user == null) return BadRequest("Email not exists -");
            Console.WriteLine("Email_in_databse--" + user.Email);
            Console.WriteLine("email_come_from--" + e.Email);

            // Implement the logic of sending email to user, to that particular mail
            var rand = new Random();
            var otp = rand.Next(100000, 1000000);
            OtpStore.EmailOtpMap[e.Email] = otp; // ✅ Save OTP per user
            // Corrected the usage of File.ReadAllText to avoid conflict with ControllerBase.File method
            string template = System.IO.File.ReadAllText("C:/Users/divan/OneDrive/Desktop/jwt_api_mvc/webApi/Template/mail_temp.html");
            string emailBody = template.Replace("123456", otp.ToString());
            var emailService = new EmailService();
            emailService.SendCustomEmail(e.Email, emailBody).Wait(); 
            // Logic for sending email can be added here

            return Ok();
        }

        [HttpPost("Forgot_Password_otpcheck")]
        public IActionResult Forgot_Password_otpcheck(mailotp temp)
        {
            //var user = _context.Users.FirstOrDefault(u => u.Email == temp.Email);
            var user = _context.Users.FirstOrDefault(u => u.Email == temp.Email);
            if (user == null) return BadRequest("User not found");

            if (!OtpStore.EmailOtpMap.TryGetValue(temp.Email, out var savedOtp))
                return BadRequest("OTP expired or not sent");

            int enteredOtp = Convert.ToInt32(temp.Otp);
            if (enteredOtp != savedOtp)
                return BadRequest("Incorrect OTP");

            return Ok("OTP verified");
        }
        [HttpPost("SetNewPassword")]
        public IActionResult SetNewPassword(Login_details user)
        {
            var us = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (us == null) return BadRequest("User NOt found ");
            us.Password = user.Password;
            //_context.Update(us);
            _context.SaveChanges();
            return Ok();

        }
    }
    public class mailotp{
        public string Email { get; set; }
        public string Otp { get; set; }

    }
    public class Emailh
    {
        public string Email { get; set; }
    }

}


