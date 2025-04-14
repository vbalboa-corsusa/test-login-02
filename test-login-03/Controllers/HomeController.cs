using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using test_login_03.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace test_login_03.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; // Crea instancias de HttpClient

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory; // Cliente http
            //_httpClient.BaseAddress = new Uri("https://dragonball-api.com/api"); // URL de la API
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                //var response = await _httpClient.GetAsync("characters");
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://dragonball-api.com/api/");
                var response = await client.GetAsync("characters");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Json recibido correctamente");
                    Console.WriteLine(json);

                    var jsonObject = JObject.Parse(json);

                    var characterList = (JArray)jsonObject["items"];

                    return View(characterList);
                }
                else
                {
                    return View("Error", new ErrorViewModel { RequestId = "Error al acceder a la API" });
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = "Error al acceder a la API: " + ex.Message;
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
