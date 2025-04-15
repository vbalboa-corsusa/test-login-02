using Microsoft.AspNetCore.Mvc;

namespace test_dragonball_api.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Verificamos si el usuario está autenticado
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
} 