using SAREM.Shared.Entities;
using SAREM.Shared.Datatypes;
using System;
using System.Collections.Generic;
using System.Linq;
using SAREM.Shared.Excepciones;

namespace SAREM.DataAccessLayer
{
    public class DALAgenda: IDALAgenda
    {
        private string tenant;
        private static int MAX_PACIENTES = 10;
        private static int MAX_PACIENTES_ESPERA = 3;
        
        public DALAgenda(string tenant)
        {
            this.tenant = tenant;
        }

        //MQ
        public void agregarConsultaPaciente(string PacienteID, long ConsultaID, short ConsultaIDTurno, Boolean fueraLista)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Paciente paciente = db.pacientes.Find(PacienteID);
                if (paciente != null)
                {
                    /*
                    Consulta consulta = db.consultas
                       .Include("pacientes")
                       .Include("pacientesespera")
                       .Where(x => x.ConsultaID == ConsultaID)
                       .Single();

                    int numpacientes = db.consultas.Include("pacientes")
                                         .Where(c => c.ConsultaID == ConsultaID)
                                         .Single().pacientes.Where(p => p.PacienteID!=null).Count();
                    */
                    PacienteConsultaAgenda consultaturno = (from ct in db.consultasagendadas
                                         where ct.PacienteID == null && ct.ConsultaID == ConsultaID && ct.ConsultaIDTurno == ConsultaIDTurno
                                         select ct).Single();

                    if (consultaturno != null)
                    {
                        consultaturno.PacienteID = PacienteID;
                        consultaturno.fueralista = false;
                        consultaturno.fecharegistro = DateTime.UtcNow;
                        db.SaveChanges();


                        //int time = ((DateTime)consulta.fecha_inicio - DateTime.UtcNow).Hours;
                        /*
                        if ((numpacientes < MAX_PACIENTES) || fueraLista)
                        {

                            var conscans = db.consultascanceladas.SingleOrDefault(x => (x.ConsultaID == ConsultaID) && (x.PacienteID == PacienteID));
                            if (conscans != null)
                            {
                                db.consultascanceladas.Remove(conscans);
                                db.SaveChanges();
                            }

                            DateTime turno = consulta.fecha_inicio;
                            int intervalo = ((consulta.fecha_fin - consulta.fecha_inicio).Minutes) / MAX_PACIENTES;
                            //turno = turno.AddMinutes(minutos * numpacientes);

                            //ordeno lista
                            List<DateTime?> ordered = consulta.pacientes.OrderBy(x => x.turno).Select(x => x.turno).ToList();
                            int orderedc = ordered.Count;

                            //busco primer turno libre
                            for (int i = 0; i < MAX_PACIENTES; i++)
                            {
                                if (i < orderedc && ordered[i] == turno)
                                    turno = turno.AddMinutes(intervalo);
                                else
                                    break;
                            }


                            if (!fueraLista)
                            {
                                PacienteConsultaAgenda pca = new PacienteConsultaAgenda
                                {
                                    ConsultaID = ConsultaID,
                                    PacienteID = PacienteID,
                                    fecharegistro = DateTime.UtcNow,
                                    fueralista = false,
                                    turno = turno
                                };

                                db.consultasagendadas.Add(pca);

                            }
                            else {

                                PacienteConsultaAgenda pca = new PacienteConsultaAgenda
                                {
                                    ConsultaID = ConsultaID,
                                    PacienteID = PacienteID,
                                    fecharegistro = DateTime.UtcNow,
                                    fueralista = true,
                                    turno = null
                                };

                                db.consultasagendadas.Add(pca);
                            }
                            db.SaveChanges();

                          
                        }
                        else
                        {
                            throw new Exception("Ya existen 10 Pacientes en Consulta::" + numpacientes);
                        }*/
                    }
                    else
                        throw new Exception("No existe turno o la consulta ha sido tomada...");
                }
                else
                {
                    throw new Exception("No existe paciente");
                }
            }
        }

        //Agregar Paciente a Lista de Espera
        public void agregarConsultaPacienteEspera(string PacienteID, long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Paciente paciente = db.pacientes.Find(PacienteID);
                if (paciente != null)
                {
                    Consulta consulta = db.consultas.Find(ConsultaID);
                    if (consulta != null)
                    {
                        int numpacientes = db.consultas.Include("pacientesespera")
                                        .Where(c => c.ConsultaID == ConsultaID)
                                        .Single().pacientesespera.Count;

                        if (numpacientes < consulta.maxpacientesespera)
                        {
                            //agrego a lista de espera
                            var espera = new PacienteConsultaEspera { 
                                ConsultaID = consulta.ConsultaID,
                                PacienteID = paciente.PacienteID,
                                fecha = DateTime.UtcNow 
                            };
                            db.pacienteespera.Add(espera);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new ExcepcionMaxPacientesEspera();//Exception("Ya existen " + MAX_PACIENTES_ESPERA + " Pacientes en lista de espera");
                        }
                    }
                    else
                    {
                        throw new ExcepcionNoExisteConsulta();//Exception("No existe consulta");
                    }
                }
                else
                {
                    throw new ExcepcionNoExistePaciente();//Exception("No existe paciente");
                }

            }
        }


        //Eliminar Paciente Lista de Espera
        public void eliminarPacienteConsultaLE(string PacienteID, long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                if (db.consultas.Any(c => c.ConsultaID == ConsultaID)
                && db.pacientes.Any(p => p.PacienteID == PacienteID))
                {
                    db.pacienteespera.Remove(db.pacienteespera.Where(c => c.ConsultaID == ConsultaID && c.PacienteID == PacienteID).Single());
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("No existe consulta o paciente");
                }
            }
        }

        //Mueve los pacientes de la LE a la consulta
        public void moverPacientesLEConsulta(List<string> pacientesIDs, long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                List<PacienteConsultaEspera> listEspera = new List<PacienteConsultaEspera>();

                foreach (string PacienteID in pacientesIDs)
                {
                    if (db.consultas.Any(c => c.ConsultaID == ConsultaID)
                        && db.pacientes.Any(p => p.PacienteID == PacienteID))
                    {
                        PacienteConsultaEspera pEspera = db.pacienteespera.First(i => (i.PacienteID == PacienteID) && (i.ConsultaID == ConsultaID));
                        listEspera.Add(pEspera);
                    }
                    else
                    {
                        throw new Exception("No existe consulta o paciente");
                    }
                }

                //Ordeno por fecha de registro de manera ascendiente
                var pacientesListOrdered = listEspera.OrderBy(x => x.fecha).ToList();

                foreach (PacienteConsultaEspera pEspera in pacientesListOrdered)
                {
                    this.eliminarPacienteConsultaLE(pEspera.PacienteID, pEspera.ConsultaID);
                    Consulta consulta = db.consultas.Find(ConsultaID);
                    int numpacientes = db.consultas.Include("pacientes")
                                         .Where(c => c.ConsultaID == ConsultaID)
                                         .Single().pacientes.Count;

                    if (numpacientes < MAX_PACIENTES)
                    {
                        PacienteConsultaAgenda pca = new PacienteConsultaAgenda { ConsultaID = ConsultaID, PacienteID = pEspera.PacienteID, fecharegistro = DateTime.UtcNow };
                        db.consultasagendadas.Add(pca);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Ya existen 10 Pacientes en Consulta");
                    }

                }
            }
            
        }

        public void cancelarConsultaPaciente(string PacienteID, long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Consulta consulta = db.consultas
                                    .Include("pacientesespera.paciente")
                                    .SingleOrDefault(x => x.ConsultaID == ConsultaID);
                Paciente paciente = db.pacientes.SingleOrDefault(x => x.PacienteID == PacienteID);

                if (consulta!=null && paciente!=null)
                {
                    var conscans = db.consultascanceladas.SingleOrDefault(x => (x.ConsultaID == ConsultaID) && (x.PacienteID == PacienteID));
                    if (conscans == null)
                    {
                        var cancelar = new PacienteConsultaCancelar
                        {
                            ConsultaID = ConsultaID,
                            PacienteID = PacienteID,
                            fecha = DateTime.UtcNow
                        };
                        db.consultascanceladas.Add(cancelar);
                        db.SaveChanges();
                    }

                    //extraer de la lista de consultas
                    PacienteConsultaAgenda pca = db.consultasagendadas
                        .Where(c => c.ConsultaID == ConsultaID && c.PacienteID == PacienteID)
                        .Single();
                    //db.consultasagendadas.Remove(pca);
                    //seteo paciente en null
                    pca.PacienteID = null;
                    

                    //busco el primero de la lista de espera y lo agrego al turno de la consulta cancelada.
                    if (consulta.pacientesespera.Count > 0)
                    {
                        PacienteConsultaEspera pce = consulta.pacientesespera
                              .OrderByDescending(x => (x.paciente.FuncionarioID != null && x.paciente.FuncionarioID == consulta.FuncionarioID))
                              .ThenBy(x => x.fecha).Select(p => p).First();

                        pca.PacienteID = pce.PacienteID;
                        pca.fecharegistro = DateTime.UtcNow;

                        //Paciente first = pce.paciente;
                        /*
                        //Creo consulta y la agrego
                        PacienteConsultaAgenda pcaadd = new PacienteConsultaAgenda {
                            ConsultaID = consulta.ConsultaID,
                            PacienteID = first.PacienteID,
                            turno = pca.turno,
                            fueralista=false,
                            fecharegistro= DateTime.UtcNow,
                            ausencia= false                           
                        };
                        db.consultasagendadas.Add(pcaadd);
                         * */
                        //remuevo de lista de espera
                        db.pacienteespera.Remove(pce);
                    }
                    //TODO Notificar Paciente
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("No existe consulta o paciente");
                }
            }
        }

        public void ausenciaConsultaPaciente(string PacienteID, long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from ca in db.consultasagendadas
                            where ca.PacienteID == PacienteID && ca.ConsultaID==ConsultaID
                            select ca;
                PacienteConsultaAgenda pca = query.Single();
                pca.ausencia = true;
                db.SaveChanges();
            }
        }

        //Precondiciones: No tiene.
        public void agregarConsulta(Consulta consulta)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                db.consultas.Add(consulta);
                db.SaveChanges();
                //creo los turnos
                DateTime turno = consulta.fecha_inicio;
                //Console.WriteLine(consulta.fecha_inicio.ToString()+consulta.fecha_fin.ToString()+"  "+ consulta.numpacientes);
                double intervalo = ((consulta.fecha_fin - consulta.fecha_inicio).TotalMinutes) / consulta.numpacientes;
                //turno = turno.AddMinutes(minutos * numpacientes);
                //Console.WriteLine((consulta.fecha_fin - consulta.fecha_inicio).Minutes+"EL INTERVALO ES "+intervalo);
                for (short i = 0; i < consulta.numpacientes; i++)
                {
                    PacienteConsultaAgenda pca = new PacienteConsultaAgenda 
                    { 
                        ConsultaID = consulta.ConsultaID,
                        ConsultaIDTurno = i,
                        ausencia = false,
                        fueralista = false,
                        turno = turno
                    };                  
                    db.consultasagendadas.Add(pca);
                    turno = turno.AddMinutes(intervalo);
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
        }

        //TODO: MAIL.
        public void modificarConsulta(Consulta consulta)
        {

            using (var db = SARMContext.getTenant(tenant))
            {
                if (!db.consultas.Any(c => c.ConsultaID == consulta.ConsultaID))
                    throw new Exception("No existe consulta");
                else
                {
                    var c = db.consultas.Find(consulta.ConsultaID);
                    c.LocalID = consulta.LocalID;
                    c.EspecialidadID = consulta.EspecialidadID;
                    //no modificable
                    //c.fecha_fin = consulta.fecha_fin;
                    //c.fecha_inicio = consulta.fecha_inicio;
                    c.FuncionarioID = consulta.FuncionarioID;
                    db.SaveChanges();
                }
            }
        }

        //TODO: MAIL.
        public void eliminarConsulta(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                if (!db.consultas.Any(c => c.ConsultaID == ConsultaID))
                    throw new Exception("No existe consulta");
                else
                {
                    var c = db.consultas.Find(ConsultaID);
                    db.consultas.Remove(c);
                    db.SaveChanges();
                }
            }
        }

        public ICollection<Consulta> listarConsultas()
        {
            //Entidades sin objetos dependencia 'datatypes'
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from c in db.consultas.Include("especialidad")
                            .Include("medico")
                            .Include("local")
                            select c;
                return query.ToList();
            }
        }

        public ICollection<Consulta> listarConsultasPaciente(string PacienteID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var p = db.pacientes.Find(PacienteID);
                if (p == null)
                    throw new Exception("No existe paciente");

                var q = from c in db.consultas.Include("pacientes.paciente")
                        .Include("local")
                        .Include("medico")
                        .Include("especialidad")
                        where c.pacientes.Any(x => x.PacienteID == PacienteID)
                        select c;
                return q.ToList();
               
            }
        }

        public ICollection<Consulta> listarConsultasCanceladasPaciente(string PacienteID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                if (!db.pacientes.Any(p => p.PacienteID == PacienteID))
                    throw new Exception("No existe paciente");

                var q = from c in db.consultascanceladas.Include("consulta")
                        where c.PacienteID == PacienteID
                        select c.consulta;
                return q.ToList();
            }
        }

        public ICollection<Consulta> listarConsultasAusentesPaciente(string PacienteID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var q = from c in db.consultasagendadas.Include("consulta")
                        where c.PacienteID == PacienteID && c.ausencia
                        select c.consulta;
                return q.ToList();
            }
        }

        public ICollection<Funcionario> listarFuncionarios()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                return db.funcionarios.ToList();
            }
        }


        public Consulta obtenerConsulta(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from c in db.consultas.Include("especialidad")
                            .Include("medico")
                            .Include("local")
                            .Include("pacientes")
                            .Include("pacientesespera")
                            where c.ConsultaID == ConsultaID
                            select c;
                Consulta con = query.Single();
                con.pacientesespera = con.pacientesespera.ToList();
                return con;
            }
        }


        public ICollection<Paciente> listarPacientesNotInConsulta(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                if (!db.consultas.Any(c => c.ConsultaID == ConsultaID))
                    throw new Exception("No existe consulta");
                else
                {
                    var result = (from p in db.pacientes
                                  where (!db.consultasagendadas.Any(c => (c.ConsultaID == ConsultaID) && (c.PacienteID == p.PacienteID)) &&
                                  !db.pacienteespera.Any(c => (c.ConsultaID == ConsultaID) && (c.PacienteID == p.PacienteID)))
                                  select p).ToList();
                    return result;
                }
            }
        }

        public ICollection<Paciente> obtenerPacientesConsulta(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var pacientesOrdered = db.consultasagendadas.Include("paciente").Include("consulta").Where(x => (x.ConsultaID == ConsultaID && x.PacienteID!=null && x.fueralista == false))
                    .OrderBy(x=>x.turno).Select(p=>p.paciente).ToList();
                return pacientesOrdered;
            }          
        }

        public ICollection<Paciente> obtenerPacientesConsultaEspera(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var pacientesOrdered = db.pacienteespera.Include("paciente").Include("consulta").Where(x => x.ConsultaID == ConsultaID)
                        .OrderByDescending(x => (x.paciente.FuncionarioID != null && x.paciente.FuncionarioID == x.consulta.FuncionarioID))
                        .ThenBy(x => x.fecha).Select(p => p.paciente).ToList();
                return pacientesOrdered;
            }
        }

        public ICollection<Paciente> obtenerPacientesConsultaFueraLista(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                //no es necesario ordenar este caso.
                var pacientesOrdered = db.consultasagendadas.Include("paciente").Include("consulta").Where(x => (x.PacienteID!=null && x.ConsultaID == ConsultaID && x.fueralista == true))
                    .Select(p => p.paciente).ToList();
                    //.OrderByDescending(x => (x.paciente.FuncionarioID != null && x.paciente.FuncionarioID == x.consulta.FuncionarioID))
                    //.ThenBy(x => x.fecharegistro).Select(p => p.paciente).ToList();
                return pacientesOrdered;
            }
        }


        // Por definicion el parte diario corresponde a la consulta de un medico
        // dada una fecha.
        public ICollection<Consulta> obtenerParteDiario(string MedicoID, DateTime fecha)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                if (fecha.Date <= DateTime.UtcNow.Date)
                {
                    var query = (from c in db.consultas.Include("especialidad")
                            .Include("medico")
                            .Include("local")
                            .Include("pacientes.paciente")
                                where c.FuncionarioID == MedicoID //&& c.fecha_inicio.Date == fecha.Date
                                select c).ToList();
                    List<Consulta> lista = new List<Consulta>();
                    foreach (var con in query) {
                        if (con.fecha_inicio.Date == fecha.Date)
                            lista.Add(con);
                    }
                    return lista;
                }
                else
                {
                    throw new ExcepcionFueraDeFechaParteDiario();//Exception("El parte diario para dicha fecha no existe");
                }
            }
        }

        public void actualizarParteDiario(long ConsultaID, string PacienteID, string diagnostico, bool ausencia)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from ca in db.consultasagendadas
                            where ca.ConsultaID == ConsultaID && ca.PacienteID == PacienteID
                            select ca;
                Console.WriteLine("Paciente" + PacienteID + "consulta" + ConsultaID);
                PacienteConsultaAgenda pca = query.Single();
                pca.diagnostico = diagnostico;
                pca.ausencia = ausencia;
                db.Entry(pca).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public PacienteConsultaAgenda obtenerPacienteConsulta(long ConsultaID, string PacienteID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from c in db.consultasagendadas
                            where c.ConsultaID == ConsultaID && c.PacienteID == PacienteID
                            select c;
                return query.Single();              
            }

        }
        /*
        public DataParametros obtenerParametrosConsulta()
        {
            DataParametros d = new DataParametros();
            d.maxPacientesConsulta = MAX_PACIENTES;
            d.maxPacientesListaEspera = MAX_PACIENTES_ESPERA;
            return d;
        }*/

        public ICollection<Consulta> listarConsultasMedicoLocalEspecialidad(long EspecialidadID, long LocalID, string MedicoID, DateTime fecha, string idP)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from c in db.consultas.Include("pacientes")
                            .Include("especialidad")
                            .Include("medico")
                            .Include("local")
                            where c.LocalID == LocalID 
                            && c.EspecialidadID == EspecialidadID 
                            && c.FuncionarioID == MedicoID
                            && !c.pacientes.Any(p => p.PacienteID == idP )
                            && !c.pacientesespera.Any(p => p.PacienteID == idP)
                            && (c.pacientes.Count < MAX_PACIENTES || c.pacientesespera.Count < MAX_PACIENTES_ESPERA)
                            select c;
                List<Consulta> lista = new List<Consulta>();
                foreach (var c in query.ToList())
                {
                    if (c.fecha_inicio.Date == fecha.Date)
                        lista.Add(c);
                }
                return lista;
            }
        }


        // en el controlador hacer un try catch con ExcepcionMaxPacientesEspera, si se captura este caso notificar
        // que no existe lugar.
        // Si devuelve un datetime se agendo consulta con exito y se notifica la hora con dicho datetime, 
        // en caso que sea null, se agrego a la lista de espera.
        public DateTime? agregarConsultaPaciente(string PacienteID, long ConsultaID, short ConsultaIDTurno)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Paciente paciente = db.pacientes.Find(PacienteID);
                if (paciente != null)
                {
                    //Consulta consulta =
                        //db.consultas
                        //.Include("pacientes")
                        //.Include("pacientesespera")
                        //.Where(x=> x.ConsultaID==ConsultaID)
                        //.Single();
                    //int numpacientes = db.consultas.Include("pacientes").Include("pacientesespera")
                    //                     .Where(c => c.ConsultaID == ConsultaID)
                    //                     .Single().pacientes.Count;
                    //int numpacientes = consulta.pacientes.Count;

                    //controlo que paciente no tenga un turno en la consulta
                    var pcaexiste = from p in db.consultasagendadas
                                                  where p.ConsultaID == ConsultaID && p.PacienteID == PacienteID
                                                  select p;
                    if (pcaexiste.Count() > 0)
                    {
                        throw new Exception("Paciente ya tiene un turno para esta consulta...");
                    }

                    PacienteConsultaAgenda pca = (from p in db.consultasagendadas
                                                  where p.ConsultaIDTurno == ConsultaIDTurno &&
                                                  p.ConsultaID == ConsultaID && p.paciente==null
                                                  select p).Single();
                    if (pca != null)
                    {
                        pca.PacienteID = PacienteID;
                        pca.fecharegistro = DateTime.UtcNow;
                        db.SaveChanges();
                        return pca.turno;
                    }
                    else
                    {
                        throw new Exception("Consulta no existe o turno tomado...");
                    }

                    /*
                    if (consultaturno != null)
                    {

                        
                        if ((numpacientes < consulta.numpacientes))
                        {
                            var conscans = db.consultascanceladas
                                .SingleOrDefault(x => (x.ConsultaID == ConsultaID)
                                && (x.PacienteID == PacienteID));
                            if (conscans != null)
                            {
                                db.consultascanceladas.Remove(conscans);
                                db.SaveChanges();
                            }
                            DateTime turno = consulta.fecha_inicio;
                            int intervalo = ((consulta.fecha_fin - consulta.fecha_inicio).Minutes) / MAX_PACIENTES;
                            //turno = turno.AddMinutes(minutos * numpacientes);
                           
                            //ordeno lista
                            List<DateTime ?> ordered = consulta.pacientes.OrderBy(x => x.turno).Select(x=> x.turno).ToList();
                            int orderedc = ordered.Count;

                            //busco primer turno libre
                            for (int i = 0; i < MAX_PACIENTES; i++)
                            {
                                if (i < orderedc && ordered[i] == turno)
                                    turno = turno.AddMinutes(intervalo);
                                else
                                    break;                                                        
                            }
                            
                            PacienteConsultaAgenda pca = new PacienteConsultaAgenda 
                            {   
                                ConsultaID = ConsultaID,
                                PacienteID = PacienteID, 
                                fecharegistro = DateTime.UtcNow, 
                                fueralista = false,
                                turno = turno
                            };
                            db.consultasagendadas.Add(pca);
                            db.SaveChanges();
                            return turno;
                        }
                        else
                        {
                            agregarConsultaPacienteEspera(PacienteID, ConsultaID);
                            return null;
                        }
                    }
                    else
                        throw new ExcepcionNoExisteConsulta();//Exception("No existe consulta");
                     * */
                }
                else
                {
                    throw new ExcepcionNoExistePaciente();//Exception("No existe paciente");
                }
            }

        }


        
        public PacienteConsultaAgenda obtenerConsulta(string PacienteID, long ConsultaID)
        {

            using (var db = SARMContext.getTenant(tenant))
            {
                var q = from cs in db.consultasagendadas
                        where (cs.ConsultaID == ConsultaID && cs.PacienteID == PacienteID)
                        select cs;


                if (q != null)
                {
                    var consulta = q.Single();
                    return consulta;
                }
                else
                {
                    return null;
                }
            }
        }

        public Boolean perteneceConsulta(string PacienteID, long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
               if (db.consultasagendadas.Any(c => c.ConsultaID == ConsultaID && c.PacienteID == PacienteID)) {
                   
                   return true;
               }
               else
               {
                   return false;
               }
            }
        }

        public ICollection<PacienteConsultaAgenda> obtenerTurnosLibres(long ConsultaID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                //si es necesario realizar los include pertinentes
                var query = from t in db.consultasagendadas
                            where t.ConsultaID == ConsultaID && t.PacienteID == null
                            select t;
                return query.ToList();
            }

        }

    }
}
