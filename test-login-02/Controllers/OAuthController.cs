using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace test_login_02.Controllers
{
    public class OAuthController : Controller
    {
        private readonly HttpClient _client = new HttpClient(); //Cliente http
        private readonly BitrixSettings _settings;

        //private string ClientId = "5728"; //ID de cliente
        //private string ClientSecret = "ooyssbe5leg0hl3t"; //Secreto de cliente
        //private string RedirectUri = "https://cc7d-38-187-9-209.ngrok-free.app/oauth/callback"; //URL handler path
        // private string redirectUri = "https://corsusaint.bitrix24.com/rest/oauth/redirect.php"; //URL de redirección

        public OAuthController(IOptions<BitrixSettings> settings)
        {
            _settings = settings.Value;
        }

    //public IActionResult Install()
    //{
    //    ViewBag.Message = "Aplicación instalada correctamente"; //Mensaje de instalación
    //    return View(); //Retorno de la vista
    //}

    // Método para redirigir a la URL de autorización
    public IActionResult Authorize()
        {
            string authorizationUrl = $"https://corsusaint.bitrix24.com/oauth/authorize/?client_id={_settings.ClientId}&response_type=code&redirect_uri={_settings.RedirectUri}"; //URL de autorización
            return Redirect(authorizationUrl); //Redirigir a la URL de autorización
        }

        public async Task<ActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Message = "Código de autorización no disponible.";
                return View("Index");
            }

            string tokenUrl = "https://corsusaint.bitrix24.com/rest/5728/ooyssbe5leg0hl3t/user.current";

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _settings.ClientId),
                new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _settings.RedirectUri)                
            });

            try
            {
                var response = await _client.PostAsync(tokenUrl, requestBody);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(content);
                    string accessToken = jsonResponse["access_token"]?.ToString();

                    if (accessToken != null)
                    {
                        ViewBag.Message = "Acceso correcto, token " + accessToken;
                        HttpContext.Session.SetString("access_token", accessToken); //Guardar el token de acceso en la sesión
                    }
                    else
                    {
                        ViewBag.Message = "Error al obtener el token de acceso.";
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
