using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers;

public class HomeController : Controller
{
    private readonly EventoRepositorio _repo;

    public HomeController(EventoRepositorio repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _repo.ResumenInicioAsync());
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
