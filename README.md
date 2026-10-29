# Eventos.Web

Aplicación ASP.NET Core MVC (.NET 10) de eventos e inscripciones con **Entity Framework Core Code First** y SQL Server. Laboratorio SEMANA 11.

## Características
- Base de datos `EventosDB` creada solo con migraciones (sin scripts SQL, sin `EnsureCreated`, sin Fluent API).
- Entidades con Data Annotations: `Categoria`, `Evento`, `Asistente`, `Inscripcion`.
- Eliminación lógica (`Activo = false`) y cancelación de inscripciones (`Estado = "Cancelada"`).
- Patrón repositorio (`EventoRepositorio`, `AsistenteRepositorio`, `InscripcionRepositorio`) con consultas LINQ async y `AsNoTracking()`.
- Validaciones de inscripción: evento no pasado, asistente no duplicado y aforo disponible.
- Datos iniciales (3 categorías, 4 eventos, 5 asistentes) cargados desde `Program.cs` solo si las tablas están vacías.

## Requisitos
- .NET SDK 10
- SQL Server / SQL Server Express
- `dotnet tool install --global dotnet-ef`

## Configuración
Ajusta la cadena de conexión en `appsettings.json` si tu instancia no es `.\SQLEXPRESS`:

```json
"ConnectionStrings": {
  "EventosDB": "Server=.\\SQLEXPRESS;Database=EventosDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

## Ejecución
```bash
dotnet ef database update
dotnet run
```

## Migraciones (3)
| Migración | Cambio |
|---|---|
| `InitialCreate` | Cuatro entidades y relaciones |
| `AgregarModalidadEvento` | `Evento.Modalidad` (default `"Presencial"`) |
| `AgregarTelefonoAsistente` | `Asistente.Telefono` (opcional, máx. 9) |

Comandos usados:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations add AgregarModalidadEvento
dotnet ef database update
dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

Verificación:
```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```
