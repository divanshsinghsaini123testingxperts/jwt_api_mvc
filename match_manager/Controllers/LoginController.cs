using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Pkcs;

namespace match_manager.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult MainView()
        {
            return View("MainView");
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }


    }
}
