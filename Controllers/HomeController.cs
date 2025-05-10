using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Menus.Models;
using System.Text.Json;
using Menus.Services;

namespace Menus.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient httpClient;
    private readonly IAuthService authService;
    public HomeController(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.authService = new SimpleAuthService();

    }


    [HttpGet("Home/GetPokemon/{name}")]
    public async Task<IActionResult> GetPokemon(string name)
    {
        String url = $"https://pokeapi.co/api/v2/pokemon/{name}";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }
        String json = await response.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(json);
        Pokemon pokemon = new Pokemon
        {
            Name = doc.RootElement.GetProperty("name").GetString(),
            ImageUrl = doc.RootElement.GetProperty("sprites").GetProperty("front_default").GetString(),
            Types = doc.RootElement.GetProperty("types").EnumerateArray().Select(t => t.GetProperty("type").GetProperty("name").GetString()).ToList(),
            Abilities = doc.RootElement.GetProperty("abilities").EnumerateArray().Select(a => a.GetProperty("ability").GetProperty("name").GetString()).ToList(),
            Weight = doc.RootElement.GetProperty("weight").GetInt32(),
        };

        return View(pokemon);
    }

    [HttpGet("Home/Getjoke")]
    public async Task<ActionResult> GetJoke()
    {
        String url = "https://v2.jokeapi.dev/joke/Programming?type=twopart";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }
        String json = await response.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(json);
        Joke joke = new Joke
        {
            //Error = doc.RootElement.GetProperty("error").GetString(),
            Type = doc.RootElement.GetProperty("type").GetString(),
            //Category = doc.RootElement.GetProperty("category").GetString(),
            Setup = doc.RootElement.GetProperty("setup").GetString(),
            Delivery = doc.RootElement.GetProperty("delivery").GetString(),
            Safe = doc.RootElement.GetProperty("safe").GetBoolean(),
            //Id = doc.RootElement.GetProperty("id").GetInt32(),
            //Lang = doc.RootElement.GetProperty("lang").GetString()
        };
        return View(joke);
    }

    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult LogIn(User user)
    {
        if (authService.Authenticate(user.Username, user.Password))
        {
            HttpContext.Session.SetString("UserAuthenticated", "true");
            return RedirectToAction("GetJoke");
        }
        else
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Photos()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult PaginaS()
    {
        var isAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
        if (isAuthenticated != "true")
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
