using Microsoft.ServiceBus.Messaging;
using SAREM.Shared.Datatypes;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
            EventoOpcional e = db.eventos.Include("rangos").OfType<EventoOpcional>().Where(ev => ev.EventoID == EventoID).First();
            Paciente p = db.pacientes.Find(PacienteID);
            Comunicacion c = db.comunicaciones.Find(ComunicacionID);

            if(e!=null && p!=null && c!=null)
            {
                DateTime now = DateTime.Today;
                int edad = now.Year - p.FN.Year;
                if (p.FN > now.AddYears(-edad)) edad--;

                if (e.edades.Contains(edad) && e.sexo == p.sexo)
                {
                    EventoPacienteComunicacion epc = new EventoPacienteComunicacion { ComunicacionID = ComunicacionID, 
                                                                                      EventoID = EventoID, 
                                                                                      PacienteID = PacienteID };
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
        public ICollection<Evento> listarEventosOpcionales(string PacienteID)
        {
            List<Evento> eventos = new List<Evento>();
            Paciente p = db.pacientes.Find(PacienteID);
            if (p != null)
            {
                var eventosop = db.eventos.OfType<EventoOpcional>().ToList(); 
                foreach (var e in eventosop)
                {
                    if (e.edades.Contains(p.FN.Year))
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

        public ICollection<Evento> listarEventosSuscriptoPaciente(string PacienteID)
        {
            var query = from p in db.eventopacientecomunicacion.Include("evento")
                        where p.PacienteID == PacienteID
                        select p.evento;
            return query.ToList();
        }

        public ICollection<Comunicacion> listarComunicaciones()
        {
            return db.comunicaciones.ToList();
        }

        public void agregarNotificacionConsulta(DataNotificacionConsulta dnc)
        {
            //inserto en el bus
            throw new NotImplementedException();
            /*
            string QueueName = "notificaciones";
            string connectionString = ConfigurationManager.ConnectionStrings["sarembus"].ConnectionString;
            QueueClient Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            var message = new BrokeredMessage(dnc) { ScheduledEnqueueTimeUtc =dnc.fecha };
            Client.Send(message);*/
        }

   
        //Obtener todas las notificaciones
        public ICollection<Evento> listarEventos()
        {
            //Entidades sin objetos dependencia 'datatypes'
            var query = from e in db.eventos
                        select e;
            return query.ToList();
        }

        #region CRUD Eventos
        //crud eventos
        public Evento obtenerEvento(long EventoID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                return db.eventos.Find(EventoID);
            }
        }
        
        public void crearEvento(Evento e)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                db.eventos.Add(e);
                db.SaveChanges();
            }
        }
        
        public void eliminarEvento(long EventoID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Evento e = db.eventos.Find(EventoID);
                if (e != null)
                {
                    db.eventos.Remove(e);
                    db.SaveChanges();
                }
            }
        }
        
        public void modificarEvento(Evento evento)
        {
            db.eventos.Attach(evento);
            db.Entry<Evento>(evento).State = EntityState.Modified;
            db.SaveChanges();
        }
        #endregion 


    }
}
