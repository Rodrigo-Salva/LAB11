using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Inscripcion
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Evento")]
    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    public Evento? Evento { get; set; }

    [Display(Name = "Asistente")]
    public int AsistenteId { get; set; }

    [ForeignKey(nameof(AsistenteId))]
    public Asistente? Asistente { get; set; }

    [Display(Name = "Fecha de inscripción")]
    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = "Confirmada";
}
