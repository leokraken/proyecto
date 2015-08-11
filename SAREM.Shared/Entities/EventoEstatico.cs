
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SAREM.Shared.Entities
{
    public class EventoEstatico : Evento
    {
        [Required]
        public virtual ICollection<Rango> rangos { get; set; }
    }
}
