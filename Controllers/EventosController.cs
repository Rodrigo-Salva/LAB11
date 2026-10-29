using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _repo;

    public EventosController(EventoRepositorio repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = await _repo.CategoriasActivasAsync(categoriaId);
        return View(await _repo.ListarActivosAsync(categoriaId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var evento = await _repo.ObtenerAsync(id);
        if (evento == null) return NotFound();

        ViewBag.Inscritos = await _repo.ContarInscritosAsync(id);
        return View(evento);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync(null, null);
        return View(new Evento());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        ModelState.Remove(nameof(Evento.Categoria));
        if (ModelState.IsValid)
        {
            await _repo.CrearAsync(evento);
            TempData["Mensaje"] = "Evento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _repo.ObtenerAsync(id);
        if (evento == null) return NotFound();

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id) return NotFound();

        ModelState.Remove(nameof(Evento.Categoria));
        if (ModelState.IsValid)
        {
            if (!await _repo.ActualizarAsync(evento)) return NotFound();

            TempData["Mensaje"] = "Evento actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _repo.ObtenerAsync(id);
        if (evento == null) return NotFound();

        return View(evento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await _repo.EliminarLogicoAsync(id)) return NotFound();

        TempData["Mensaje"] = "Evento eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? categoriaId, string? modalidad)
    {
        ViewBag.Categorias = await _repo.CategoriasActivasAsync(categoriaId);
        ViewBag.Modalidades = _repo.ModalidadesLista(modalidad);
    }
}
