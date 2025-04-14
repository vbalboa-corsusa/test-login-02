using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using test_login_03.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace test_login_03.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DragonballSettings _settings;

        public AuthController(IHttpClientFactory httpClientFactory, IOptions<DragonballSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }

        public IActionResult Authorize()
        {
            return RedirectToAction("LoginWithGitHub");
        }

        [Route("github/login")]
        public IActionResult LoginWithGitHub()
        {
            var clientId = _settings.ClientId;
            var redirectUri = _settings.RedirectUri;
            var oauthUrl = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=read:user%20user:email";

            return Redirect(oauthUrl);
        }

        [HttpGet("github/callback")]
        public async Task<IActionResult> GitHubCallBack(string code)
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
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");

            var tokenResponse = await client.SendAsync(tokenRequest);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenJson = JObject.Parse(tokenContent);
            var accessToken = tokenJson["access_token"]?.ToString();

            if (string.IsNullOrEmpty(accessToken))
                return BadRequest("Failed to get access token.");

            var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            userRequest.Headers.UserAgent.ParseAdd("DragonballApp");

            var userResponse = await client.SendAsync(userRequest);
            var userJson = JObject.Parse(await userResponse.Content.ReadAsStringAsync());

            HttpContext.Session.SetString("UserName", userJson["login"]?.ToString());

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
