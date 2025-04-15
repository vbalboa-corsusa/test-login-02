using Microsoft.AspNetCore.Mvc;

namespace test_dragonball_api.Controllers
{
  public class RazasController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
