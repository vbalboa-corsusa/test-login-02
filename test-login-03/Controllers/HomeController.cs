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
            _httpClientFactory = httpClientFactory; //Cliente http
            //_httpClient.BaseAddress = new Uri("https://dragonball-api.com/api"); //URL de la API
        }
        //Comentario de prueba. Volviendo a mandar

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
                    //return View(model: null);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = "Error al acceder a la API: " + ex.Message;
                return View();
            }
        }
    }
}
