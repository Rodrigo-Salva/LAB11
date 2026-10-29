using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoItem
{
    public Evento Evento { get; set; } = null!;
    public string Categoria { get; set; } = string.Empty;
    public int Inscritos { get; set; }
    public int Cupos => Evento.Aforo - Inscritos;
}

public class ResumenInicio
{
    public int Eventos { get; set; }
    public int Asistentes { get; set; }
    public int Inscripciones { get; set; }
    public int Categorias { get; set; }
    public List<EventoItem> Proximos { get; set; } = new();
}

public class EventoRepositorio
{
    public static readonly string[] Modalidades = { "Presencial", "Virtual" };

    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<EventoItem>> ListarActivosAsync(int? categoriaId)
    {
        var query = _context.Eventos.AsNoTracking()
            .Where(e => e.Activo);

        if (categoriaId.HasValue)
            query = query.Where(e => e.CategoriaId == categoriaId.Value);

        return await query
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoItem
            {
                Evento = e,
                Categoria = e.Categoria!.Nombre,
                Inscritos = e.Inscripciones.Count(i => i.Estado == "Confirmada")
            })
            .ToListAsync();
    }

    public async Task<ResumenInicio> ResumenInicioAsync()
    {
        var hoy = DateTime.Today;
        return new ResumenInicio
        {
            Eventos = await _context.Eventos.AsNoTracking().CountAsync(e => e.Activo),
            Asistentes = await _context.Asistentes.AsNoTracking().CountAsync(a => a.Activo),
            Categorias = await _context.Categorias.AsNoTracking().CountAsync(c => c.Activo),
            Inscripciones = await _context.Inscripciones.AsNoTracking().CountAsync(i => i.Estado == "Confirmada"),
            Proximos = await _context.Eventos.AsNoTracking()
                .Where(e => e.Activo && e.Fecha >= hoy)
                .OrderBy(e => e.Fecha)
                .Take(3)
                .Select(e => new EventoItem
                {
                    Evento = e,
                    Categoria = e.Categoria!.Nombre,
                    Inscritos = e.Inscripciones.Count(i => i.Estado == "Confirmada")
                })
                .ToListAsync()
        };
    }

    public async Task<Evento?> ObtenerAsync(int id) =>
        await _context.Eventos.AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);

    public async Task<int> ContarInscritosAsync(int eventoId) =>
        await _context.Inscripciones.AsNoTracking()
            .CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada");

    public async Task CrearAsync(Evento evento)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento evento)
    {
        var actual = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == evento.Id && e.Activo);
        if (actual == null) return false;

        actual.Titulo = evento.Titulo;
        actual.Fecha = evento.Fecha;
        actual.Lugar = evento.Lugar;
        actual.Aforo = evento.Aforo;
        actual.Precio = evento.Precio;
        actual.CategoriaId = evento.CategoriaId;
        actual.Modalidad = evento.Modalidad;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarLogicoAsync(int id)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento == null) return false;

        evento.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SelectList> CategoriasActivasAsync(int? seleccionada = null)
    {
        var categorias = await _context.Categorias.AsNoTracking()
            .Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync();
        return new SelectList(categorias, "Id", "Nombre", seleccionada);
    }

    public SelectList ModalidadesLista(string? seleccionada = null) =>
        new SelectList(Modalidades, seleccionada);
}
