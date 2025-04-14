using Microsoft.AspNetCore.Mvc;

namespace test_bitrix24_api.Controllers
{
  public class ContactController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
