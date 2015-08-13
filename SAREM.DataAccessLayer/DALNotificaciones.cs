using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALNotificaciones : IDALNotificaciones
    {
        private string tenant;
        private SARMContext db = null;

        public DALNotificaciones(string tenant)
        {
            this.tenant = tenant;
            db = SARMContext.getTenant(tenant);
        }

        public void suscribirPacienteEvento(long EventoID, string PacienteID, long ComunicacionID)
        {
            EventoEstatico e = db.eventos.Include("rangos").OfType<EventoEstatico>().Where(ev => ev.EventoID == EventoID).First();

            Paciente p = db.pacientes.Find(PacienteID);
            Comunicacion c = db.comunicaciones.Find(ComunicacionID);

            if(e!=null && p!=null && c!=null)
            {
                DateTime now = DateTime.Today;
                int edad = now.Year - p.FN.Year;
                if (p.FN > now.AddYears(-edad)) edad--;

                bool ok = false;
                foreach (var r in e.rangos)
                {
                    //Console.WriteLine(p.FN.Year+"::"+r.limitei + " " + r.limites);
                    if (edad >= r.limitei && edad <= r.limites && r.sexo==p.sexo)
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
        public ICollection<Evento> listarEventosPosibles(string PacienteID)
        {
            List<Evento> eventos = new List<Evento>();
            Paciente p = db.pacientes.Find(PacienteID);
            if (p != null)
            {
                List<EventoEstatico> eventosestaticos = db.eventos.Include("rangos").OfType<EventoEstatico>().ToList(); 
                foreach (var e in eventosestaticos)
                {
                    foreach (var r in e.rangos)
                    {
                        if (p.FN.Year >= r.limitei && p.FN.Year <= r.limites)
                        {
                            eventos.Add(e);
                            break;
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

        public ICollection<EventoPacienteComunicacion> listarEventosSuscriptoPaciente(string PacienteID)
        {
            var query = from p in db.eventopacientecomunicacion
                            //.Include("paciente")
                            .Include("comunicacion")
                            .Include("evento")
                        where p.PacienteID == PacienteID
                        select p;
            return query.ToList();
        }

        public ICollection<Comunicacion> listarComunicaciones()
        {
            return db.comunicaciones.ToList();
        }
    }
}
