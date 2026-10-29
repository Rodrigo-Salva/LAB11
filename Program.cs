using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Datos iniciales: se ejecutan después de aplicar las migraciones y solo si las tablas están vacías.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventosContext>();
    await db.Database.MigrateAsync();

    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.AddRange(
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Negocios" },
            new Categoria { Nombre = "Arte y Cultura" });
        await db.SaveChangesAsync();
    }

    if (!await db.Eventos.AnyAsync())
    {
        var cats = await db.Categorias.OrderBy(c => c.Id).ToListAsync();
        db.Eventos.AddRange(
            new Evento { Titulo = "Congreso de .NET", Fecha = DateTime.Today.AddDays(15), Lugar = "Auditorio Central", Aforo = 200, Precio = 50m, CategoriaId = cats[0].Id },
            new Evento { Titulo = "Taller de Inteligencia Artificial", Fecha = DateTime.Today.AddDays(30), Lugar = "Laboratorio 3", Aforo = 2, Precio = 80m, CategoriaId = cats[0].Id },
            new Evento { Titulo = "Foro de Emprendedores", Fecha = DateTime.Today.AddDays(20), Lugar = "Sala de Conferencias", Aforo = 100, Precio = 30m, CategoriaId = cats[1].Id },
            new Evento { Titulo = "Festival de Arte Digital", Fecha = DateTime.Today.AddDays(45), Lugar = "Galería Municipal", Aforo = 150, Precio = 0m, CategoriaId = cats[2].Id });
        await db.SaveChangesAsync();
    }

    if (!await db.Asistentes.AnyAsync())
    {
        db.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres Ruiz", Dni = "70123456", Correo = "ana.torres@correo.com" },
            new Asistente { Nombres = "Luis Pérez Gómez", Dni = "70234567", Correo = "luis.perez@correo.com" },
            new Asistente { Nombres = "María Flores Díaz", Dni = "70345678", Correo = "maria.flores@correo.com" },
            new Asistente { Nombres = "Carlos Rojas Vega", Dni = "70456789", Correo = "carlos.rojas@correo.com" },
            new Asistente { Nombres = "Sofía Castro León", Dni = "70567890", Correo = "sofia.castro@correo.com" });
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
