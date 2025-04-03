using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace test_login_02.Controllers
{
    public class OAuthController : Controller
    {
        private static readonly HttpClient _client = new HttpClient(); //Cliente http

        private string clienId = "5728"; //ID de cliente
        private string clientSecret = "ooyssbe5leg0hl3t"; //Secreto de cliente
        private string redirectUri = "https://localhost:44300/oauth/callback"; //URL de redirección
        // private string redirectUri = "https://corsusaint.bitrix24.com/rest/oauth/redirect.php"; //URL de redirección

        public IActionResult Authorize()
        {
            string authorizationUrl = $"https://corsusaint.bitrix24.com/oauth/authorize/?client_id={clienId}&response_type=code&redirect_uri={redirectUri}"; //URL de autorización
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
                new KeyValuePair<string, string>("client_id", clienId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri)                
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
