using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;


namespace test_login_03.Controllers
{
  public class AuthController : Controller
  {
    //public IActionResult Index()
    //{
    //  return View();
    //}

    private readonly IHttpClientFactory _httpClientFactory; //Cliente http
    private readonly DragonballSettings _settings;

    public AuthController(IHttpClientFactory httpClientFactory, IOptions<DragonballSettings> settings)
    {
      _httpClientFactory = httpClientFactory; //Cliente http
      _settings = settings.Value;
    }

    public IActionResult Authorize()
    {

    }

    [Route("github/login")]
    public IActionResult LoginWithGitHub([FromServices] IOptions<DragonballSettings> settings)
    {
      var clientId = settings.Value.ClientId;
      var redirectUri = settings.Value.RedirectUri;
      var oauthUrl = $"https://github.com/login/oauth/authorize?clien_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope=read:user%20user:email";

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

      HttpContext.Session.SetString("UserName", userJson["login"]?.ToString()); // Guarda el nombre de usuario en la sesión

      return RedirectToAction("Index", "Home");

    }
  }
}
