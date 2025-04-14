using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using test_bitrix24_api.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization; //consulta, analisis y manipulacion de datos JSON. LINQ(Idioma Consulta Integrada)

namespace test_bitrix24_api.Controllers
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
            string accessToken = HttpContext.Session.GetString("access_token"); //Recuperar el token de acceso de la sesión

            if (string.IsNullOrEmpty(accessToken)) //Método para verificar si el token de acceso es nulo o vacío
            {
                ViewBag.Message = "Token de acceso no disponible.";
                return View("Index");
            }

            string apiUrl = "https://corsusaint.bitrix24.com/rest/5728/ooyssbe5leg0hl3t/user.current";
            string urlWithToken = $"{apiUrl}?auth={accessToken}"; //URL de la API con el token de acceso

            try
            {
                var response = await _client.GetAsync(apiUrl);
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
