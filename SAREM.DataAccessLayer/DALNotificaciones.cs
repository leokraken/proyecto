using Microsoft.ServiceBus.Messaging;
using SAREM.Shared.Datatypes;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            EventoPersonalizado e = db.eventos.Include("rangos").OfType<EventoPersonalizado>().Where(ev => ev.EventoID == EventoID).First();

            Paciente p = db.pacientes.Find(PacienteID);
            Comunicacion c = db.comunicaciones.Find(ComunicacionID);

            if(e!=null && p!=null && c!=null)
            {
                DateTime now = DateTime.Today;
                int edad = now.Year - p.FN.Year;
                if (p.FN > now.AddYears(-edad)) edad--;

                bool ok = false;
                if (e is EventoSecuencial)
                {
                    EventoSecuencial e2 = db.eventos.Include("rango").OfType<EventoSecuencial>().Where(ev => ev.EventoID == EventoID).First(); 

                    if (edad >= e2.rango.limitei && edad <= e2.rango.limites && e2.rango.sexo == p.sexo)
                    {
                        ok = true;
                    }
                }
                else
                {
                    EventoAcotado ea = db.eventos.OfType<EventoAcotado>().Where(ev => ev.EventoID == EventoID).First();
                    if(ea.sexo == p.sexo)
                        ok=true;
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
                List<EventoSecuencial> eventossec = db.eventos.Include("rango").OfType<EventoSecuencial>().ToList(); 
                foreach (var e in eventossec)
                {
                    if (p.FN.Year >= e.rango.limitei && p.FN.Year <= e.rango.limites)
                    {
                        eventos.Add(e);
                        break;
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

        public void agregarNotificacionConsulta(DataNotificacionConsulta dnc)
        {
            //inserto en el bus
            string QueueName = "notificaciones";
            string connectionString = ConfigurationManager.ConnectionStrings["sarembus"].ConnectionString;
            QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            var message = new BrokeredMessage(dnc) { ScheduledEnqueueTimeUtc =dnc.fecha };
            Client.Send(message);
        }

   
        //Obtener todas las notificaciones
        public ICollection<Evento> listarEventos()
        {
            //Entidades sin objetos dependencia 'datatypes'
            var query = from e in db.eventos
                        select e;
            return query.ToList();
        }
    }
}
