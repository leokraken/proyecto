
using System.Collections.Generic;
namespace SAREM.Shared.Entities
{
    public class EventoNotificacion : Evento
    {
        public virtual ICollection<AgendaEvento> consultas { get; set; }
    }
}
