using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALNotificaciones
    {
        void suscribirPacienteEvento(long EventoID, string PacienteID, long ComunicacionID);
        //lista los eventos dado un PacienteID, segun sexo y rango de edades
        ICollection<Evento> listarEventosPosibles(string PacienteID);
        ICollection<EventoPacienteComunicacion> listarEventosSuscriptoPaciente(string PacienteID);
        ICollection<Comunicacion> listarComunicaciones();

    }
}
