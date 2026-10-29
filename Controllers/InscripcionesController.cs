using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _repo;

    public InscripcionesController(InscripcionRepositorio repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index() => View(await _repo.ListarAsync());

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync(null, null);
        return View(new Inscripcion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        ModelState.Remove(nameof(Inscripcion.Evento));
        ModelState.Remove(nameof(Inscripcion.Asistente));
        ModelState.Remove(nameof(Inscripcion.Estado));

        if (ModelState.IsValid)
        {
            var evento = await _repo.ObtenerEventoActivoAsync(inscripcion.EventoId);
            if (evento == null)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento no existe.");
            }
            else
            {
                if (evento.Fecha.Date < DateTime.Today)
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya pasó.");

                if (await _repo.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
                    ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito en este evento.");

                if (await _repo.ContarConfirmadasAsync(inscripcion.EventoId) >= evento.Aforo)
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El aforo del evento está completo.");
            }
        }

        if (ModelState.IsValid)
        {
            await _repo.CrearAsync(inscripcion);
            TempData["Mensaje"] = "Inscripción registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId);
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        TempData["Mensaje"] = await _repo.CancelarAsync(id)
            ? "Inscripción cancelada."
            : "No se pudo cancelar la inscripción.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? eventoId, int? asistenteId)
    {
        ViewBag.Eventos = await _repo.EventosActivosAsync(eventoId);
        ViewBag.Asistentes = await _repo.AsistentesActivosAsync(asistenteId);
    }
}
