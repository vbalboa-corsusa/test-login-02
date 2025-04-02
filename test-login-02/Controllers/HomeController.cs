using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using test_login_02.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization; //consulta, analisis y manipulacion de datos JSON. LINQ(Idioma Consulta Integrada)

namespace test_login_02.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient _client = new HttpClient(); //Cliente http
        public ActionResult Index()
        {
            return View(); //Retorno de la vista
        }


        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            string url = "https://corsusaint.bitrix24.com/rest/5728/ooyssbe5leg0hl3t/user.current";

            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync(); //Serializa contenido http en una cadena
                    //ViewBag.Message = "Acceso correcto: " + content; //ViewBag maneja dinamicamente los datos a la vista
                    var jsonResponse = JObject.Parse(content);

                    if (jsonResponse["result"] != null)
                    {
                        var user = jsonResponse["result"];
                        string userEmail = user["EMAIL"]?.ToString() ?? "Incorrecto"; //?? para gestionar valores nulos

                        ViewBag.Message = $"Acceso correcto. Usuario: {userEmail}";
                    }
                    else
                    {
                        ViewBag.Message = "Error al acceder a la API.";
                    }
                }
                else
                {
                    ViewBag.Message = "Error al acceder a la API.";
                }
            }
            catch
            {
                ViewBag.Message = "Error al acceder a la API.";
            }

            return View("Index");
        }

        
    }
}
