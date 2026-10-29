using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _repo;

    public AsistentesController(AsistenteRepositorio repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index() => View(await _repo.ListarActivosAsync());

    public IActionResult Create() => View(new Asistente());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (ModelState.IsValid)
        {
            if (await _repo.ExisteDniAsync(asistente.Dni))
                ModelState.AddModelError(nameof(Asistente.Dni), "Ya existe un asistente con ese DNI.");
            if (await _repo.ExisteCorreoAsync(asistente.Correo))
                ModelState.AddModelError(nameof(Asistente.Correo), "Ya existe un asistente con ese correo.");
        }

        if (ModelState.IsValid)
        {
            await _repo.CrearAsync(asistente);
            TempData["Mensaje"] = "Asistente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        return View(asistente);
    }
}
