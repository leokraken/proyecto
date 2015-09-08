using SAREM.Shared.Datatypes;
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
        ICollection<Evento> listarEventosOpcionales(string PacienteID);
        ICollection<Evento> listarEventosSuscriptoPaciente(string PacienteID);
        ICollection<Comunicacion> listarComunicaciones();
        void agregarNotificacionConsulta(DataNotificacionConsulta dnc);
        ICollection<Evento> listarEventos();

        //crud eventos
        Evento obtenerEvento(long EventoID);
        void crearEvento(Evento e);
        void eliminarEvento(long EventoID);
        void modificarEvento(Evento evento);
    }
}
