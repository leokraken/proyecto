
using System.ComponentModel.DataAnnotations;
namespace SAREM.Shared.Entities
{
    public class EventoEstatico : Evento
    {
        [Required]
        public virtual Rango rango { get; set; }
    }
}
