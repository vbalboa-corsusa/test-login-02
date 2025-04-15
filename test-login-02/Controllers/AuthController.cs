using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using test_bitrix24_api.Models;

namespace test_bitrix24_api.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BitrixSettings _settings;

        public AuthController(IHttpClientFactory httpClientFactory, IOptions<BitrixSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Redirigir a la autenticación de Bitrix24
            var clientId = _settings.ClientId;
            var redirectUri = _settings.RedirectUri;
            var oauthUrl = $"https://www.bitrix24.net/oauth/authorize/?user_lang=en&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=auth,profile&response_type=code&mode=page";

            return Redirect(oauthUrl);
        }

        [HttpGet("oauth/callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code is missing.");

            var client = _httpClientFactory.CreateClient();

            var body = new Dictionary<string, string>
            {
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret },
                { "code", code },
                { "redirect_uri", _settings.RedirectUri }
            };

            var requestContent = new FormUrlEncodedContent(body);
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth.bitrix24.com/oauth/token/");
            tokenRequest.Content = requestContent;

            var tokenResponse = await client.SendAsync(tokenRequest);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenJson = JObject.Parse(tokenContent);
            var accessToken = tokenJson["access_token"]?.ToString();

            if (string.IsNullOrEmpty(accessToken))
                return BadRequest("Failed to get access token.");

            // Guardar el token en la sesión
            HttpContext.Session.SetString("AccessToken", accessToken);

            // Obtener información del usuario
            var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.bitrix24.com/rest/user.current.json");
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userResponse = await client.SendAsync(userRequest);
            var userJson = JObject.Parse(await userResponse.Content.ReadAsStringAsync());

            HttpContext.Session.SetString("UserEmail", userJson["result"]?["EMAIL"]?.ToString());
            HttpContext.Session.SetString("UserName", userJson["result"]?["NAME"]?.ToString());

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
} 