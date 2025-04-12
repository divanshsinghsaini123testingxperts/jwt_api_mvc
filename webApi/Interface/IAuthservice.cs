using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using webApi.Models.Entity;
namespace webApi.Interface
{
    public interface IAuthservice
    {
        public IActionResult Register(User user);
        public IActionResult LoginRequest(Login_details login);
    }
}
