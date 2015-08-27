
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SAREM.Shared.Entities
{
    public class EventoSecuencial : EventoPersonalizado
    {
        [Required]
        public short intervalo { get; set; }
        public long RangoID { get; set; }
        [ForeignKey("RangoID")]
        public virtual Rango rango { get; set; }
    }
}
