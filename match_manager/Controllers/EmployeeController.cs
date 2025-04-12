using Microsoft.AspNetCore.Mvc;

namespace match_manager.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // View will use AJAX to load data
        }

        public IActionResult Add()
        {
            return View(); // View will contain form to post via AJAX
        }
        public IActionResult Edit(int id)
        {
            ViewBag.id = id;
            // Fetch employee data by id and return to view
            return View(); // View will contain form to post via AJAX
        }
    }
}
