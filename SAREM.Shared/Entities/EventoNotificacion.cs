
using System.Collections.Generic;
namespace SAREM.Shared
{
    public class EventoNotificacion : Evento
    {
        public virtual ICollection<AgendaEvento> consultas { get; set; }
    }
}
