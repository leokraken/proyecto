using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALNotificaciones
    {
        private string tenant;
        private SARMContext db = null;

        public DALNotificaciones(string tenant)
        {
            this.tenant = tenant;
            db = SARMContext.getTenant(tenant);
        }

        void suscribirPacienteEvento(long EventoID, string PacienteID, long ComunicacionID)
        {
            EventoEstatico e = (from ev in db.eventos.Include("rangos")
                       where ev.EventoID==EventoID && ev is EventoEstatico
                       select ev).First() as EventoEstatico;

            Paciente p = db.pacientes.Find(PacienteID);
            Comunicacion c = db.comunicaciones.Find(ComunicacionID);

            if(e!=null && p!=null && c!=null)
            {
                bool ok = false;
                foreach (var r in e.rangos)
                {
                    if (p.FN.Year >= r.limitei && p.FN.Year <= r.limites)
                    {
                        ok = true;
                        break;
                    }
                }
                if (ok)
                {
                    EventoPacienteComunicacion epc = new EventoPacienteComunicacion { ComunicacionID = ComunicacionID, EventoID = EventoID, PacienteID = PacienteID };
                    db.eventopacientecomunicacion.Add(epc);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("DALNotificaciones::suscribirPacienteEvento: No cumple los rangos del evento");
                }
            }
            else
            {
                throw new Exception("SuscribirPacienteEvento, no satisface condiciones");
            }
        }

        //lista los eventos dado un PacienteID, segun sexo y rango de edades
        ICollection<Evento> listarEventosPosibles(string PacienteID)
        {
            List<Evento> eventos = new List<Evento>();
            Paciente p = db.pacientes.Find(PacienteID);
            if (p != null)
            {
                List<EventoEstatico> eventosq = (from eq in db.eventos.Include("rangos")
                               where eq is EventoEstatico
                               select eq as EventoEstatico).ToList();

                foreach (var e in eventosq)
                {
                    foreach (var r in e.rangos)
                    {
                        if (p.FN.Year >= r.limitei && p.FN.Year <= r.limites)
                        {
                            eventos.Add(e);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Paciente no existe");
            }
            return eventos;
        }

        ICollection<EventoPacienteComunicacion> listarEventosSuscriptoPaciente(string PacienteID)
        {
            var query = from p in db.eventopacientecomunicacion
                            //.Include("paciente")
                            .Include("comunicacion")
                            .Include("evento")
                        where p.PacienteID == PacienteID
                        select p;
            return query.ToList();
        }

        ICollection<Comunicacion> listarComunicaciones()
        {
            return db.comunicaciones.ToList();
        }
    }
}
