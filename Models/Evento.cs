using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventos.Web.Models;

public class Evento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today.AddDays(7);

    [Required(ErrorMessage = "El lugar es obligatorio")]
    [StringLength(100)]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000")]
    public int Aforo { get; set; }

    [Range(0, 99999999, ErrorMessage = "El precio no puede ser negativo")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    [Required(ErrorMessage = "La modalidad es obligatoria")]
    [StringLength(20)]
    [RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "La modalidad debe ser Presencial o Virtual")]
    public string Modalidad { get; set; } = "Presencial";

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
