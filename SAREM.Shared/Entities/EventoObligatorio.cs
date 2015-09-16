using SAREM.Shared.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    /*Notificaciones de consultas y penalizacion*/
    public class EventoObligatorio : Evento
    {
        [Required]
        public DateTime fechanotificacion { get; set; }
    }
}
