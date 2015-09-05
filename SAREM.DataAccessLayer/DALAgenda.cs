using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAREM.DataAccessLayer
{
    public class DALAgenda: IDALAgenda
    {
        private SARMContext db = null;
        private static int DAY=24;
        private static int MAX_PACIENTES = 10;
        private static int MAX_PACIENTES_ESPERA = 3;
        
        public DALAgenda(string tenant)
        {
            db = SARMContext.getTenant(tenant);
        }

        //MQ
        public void agregarConsultaPaciente(string PacienteID, long ConsultaID)
        {
            //check if exists
            Paciente paciente = db.pacientes.Find(PacienteID);
            if (paciente!=null)
            {
                Consulta consulta = db.consultas.Find(ConsultaID);
                int numpacientes = db.consultas.Include("pacientes")
                                     .Where(c => c.ConsultaID == ConsultaID)
                                     .Single().pacientes.Count;

               
                if (consulta != null)
                {
                    //int time = ((DateTime)consulta.fecha_inicio - DateTime.UtcNow).Hours;
                  
                    if (numpacientes <= MAX_PACIENTES)
                    {

                        var conscans = db.consultascanceladas.SingleOrDefault(x => (x.ConsultaID == ConsultaID) && (x.PacienteID == PacienteID));
                        if (conscans != null)
                        {
                            db.consultascanceladas.Remove(conscans);
                            db.SaveChanges();
                        }
                    
                        PacienteConsultaAgenda pca = new PacienteConsultaAgenda { ConsultaID = ConsultaID, PacienteID = PacienteID, fecharegistro = DateTime.UtcNow };
                        db.consultasagendadas.Add(pca);
                        db.SaveChanges();
                    }
                    else
                    {
                        //agrego a lista de espera
                        //var espera = new PacienteConsultaEspera { ConsultaID=consulta.ConsultaID, PacienteID=paciente.PacienteID, fecha= DateTime.UtcNow };
                        //db.pacienteespera.Add(espera);
                        //db.SaveChanges();
                        throw new Exception("Ya existen 10 Pacientes en Consulta");
                    }
                }
                else
                    throw new Exception("No existe consulta");
                
            }
            else
            {
                throw new Exception("No existe paciente");
            }
        }

        //Agregar Paciente a Lista de Espera
        public void agregarConsultaPacienteEspera(string PacienteID, long ConsultaID)
        {
             //check if exists
            Paciente paciente = db.pacientes.Find(PacienteID);
            if (paciente != null)
            {
                Consulta consulta = db.consultas.Find(ConsultaID);
                if (consulta != null)
                {
                    int numpacientes = db.consultas.Include("pacientesespera")
                                    .Where(c => c.ConsultaID == ConsultaID)
                                    .Single().pacientesespera.Count;

                    if (numpacientes <= MAX_PACIENTES_ESPERA)
                    {
                        //agrego a lista de espera
                        var espera = new PacienteConsultaEspera { ConsultaID = consulta.ConsultaID, PacienteID = paciente.PacienteID, fecha = DateTime.UtcNow };
                        db.pacienteespera.Add(espera);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Ya existen 3 Pacientes en lista de espera");
                    }

                } else{

                    throw new Exception("No existe consulta");
                }
            }
            else
            {
                throw new Exception("No existe paciente");
            }

        }


        //Eliminar Paciente Lista de Espera
        public void eliminarPacienteConsultaLE(string PacienteID, long ConsultaID)
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

        //Mueve los pacientes de la LE a la consulta
        public void moverPacientesLEConsulta(List<string> pacientesIDs, long ConsultaID)
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
                    PacienteConsultaAgenda pca = new PacienteConsultaAgenda { ConsultaID = ConsultaID, PacienteID = pEspera.PacienteID, fecharegistro = pEspera.fecha };
                    db.consultasagendadas.Add(pca);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Ya existen 10 Pacientes en Consulta");
                }

            }
            
        }

        public void cancelarConsultaPaciente(string PacienteID, long ConsultaID)
        {
            if (db.consultas.Any(c => c.ConsultaID == ConsultaID)
                && db.pacientes.Any(p => p.PacienteID == PacienteID))
            {
                var conscans = db.consultascanceladas.SingleOrDefault(x => (x.ConsultaID == ConsultaID) && (x.PacienteID == PacienteID));
                if (conscans != null)
                {
                    db.consultascanceladas.Remove(conscans);
                    db.SaveChanges();
                }
              
                var cancelar = new PacienteConsultaCancelar {
                    ConsultaID = ConsultaID,
                    PacienteID = PacienteID,
                    fecha = DateTime.UtcNow 
                };
                db.consultascanceladas.Add(cancelar);
                //extraer de la lista de consultas
                db.consultasagendadas.Remove(db.consultasagendadas.Where(c => c.ConsultaID == ConsultaID && c.PacienteID == PacienteID).Single());
                
               
                db.SaveChanges();
            }
            else
            {
                throw new Exception("No existe consulta o paciente");
            }
        }

        public void ausenciaConsultaPaciente(string PacienteID, long ConsultaID)
        {
            var paciente = db.pacientes.Find(PacienteID);
            if (paciente!=null)
            {
                var consulta = db.consultas.Find(ConsultaID);
                if (consulta != null)
                {
                    //consulta.ausencia = paciente;
                    var a = new PacienteConsultaAusencia { ConsultaID=ConsultaID, PacienteID=PacienteID};
                    db.pacienteausentes.Add(a);
                    db.SaveChanges();
                }
            }
            else
            {
                throw new Exception("No existe paciente");
            }
        }

        //Precondiciones: No tiene.
        public void agregarConsulta(Consulta consulta)
        {
            db.consultas.Add(consulta);
            db.SaveChanges();
        }

        public void modificarConsulta(Consulta consulta)
        {
            if (!db.consultas.Any(c => c.ConsultaID == consulta.ConsultaID))
                throw new Exception("No existe consulta");
            else
            {
                var c = db.consultas.Find(consulta.ConsultaID);
                c.LocalID = consulta.LocalID;
                c.local = obtenerLocal(consulta.LocalID);
                c.EspecialidadID = consulta.EspecialidadID;
                c.especialidad = this.obtenerEspecialidad(consulta.EspecialidadID);
                c.fecha_fin = consulta.fecha_fin;
                c.fecha_inicio = consulta.fecha_inicio;
                c.FuncionarioID = consulta.FuncionarioID;
                c.medico = this.obtenerMedico(consulta.FuncionarioID);
                db.SaveChanges();
            }
        }

        public void eliminarConsulta(long ConsultaID)
        {
            if(!db.consultas.Any(c => c.ConsultaID==ConsultaID))
                throw new Exception("No existe consulta");
            else
            {
                var c = db.consultas.Find(ConsultaID);
                db.consultas.Remove(c);
                db.SaveChanges();
            }
        }

        public ICollection<Consulta> listarConsultas()
        {
            //Entidades sin objetos dependencia 'datatypes'
            var query = from c in db.consultas.Include("especialidad")
                        .Include("medico")
                        .Include("local")
                        select c;
            return query.ToList();
        }

        public ICollection<Consulta> listarConsultasPaciente(string PacienteID)
        {
            var p = db.pacientes.Find(PacienteID);
            if (p == null)
                throw new Exception("No existe paciente");
            else
            {
                var q = from c in db.consultasagendadas.Include("consulta")
                        where c.PacienteID == PacienteID
                        select c.consulta;
                return q.ToList();
            }
        }



        public ICollection<Consulta> listarConsultasCanceladasPaciente(string PacienteID)
        {
            if (!db.pacientes.Any(p => p.PacienteID == PacienteID))
                throw new Exception("No existe paciente");

            var q = from c in db.consultascanceladas.Include("consulta")
                    where c.PacienteID == PacienteID
                    select c.consulta;
            return q.ToList();
        }

        public ICollection<Consulta> listarConsultasAusentesPaciente(string PacienteID)
        {
            var q = from c in db.pacienteausentes.Include("consulta")
                    where c.PacienteID == PacienteID
                    select c.consulta;
            return q.ToList();
        }

        public ICollection<Especialidad> listarEspecialidades()
        {
            return db.especialidades.ToList();
        }

        public ICollection<Local> listarLocales()
        {
            return db.locales.ToList();
        }

        public ICollection<Funcionario> listarFuncionarios()
        {
            return db.funcionarios.ToList();
        }

        public ICollection<Especialidad> listarEspecialidadesLocal(long LocalID)
        {
            var q = from e in db.locales.Include("especialidades")
                    where e.LocalID == LocalID
                    select e;
            var local = q.First();
            if (local != null)
            {
                return local.especialidades.ToList();
            }
            else
            {
                throw new Exception("No existe especialidad");
            }
        }

        public ICollection<Medico> listarMedicosEspecialidadLocal(long LocalID, long EspecialidadID)
        {
            var medicos = db.funcionarios
                .OfType<Medico>()
                .Where(m => m.especialidades.Any(e => e.EspecialidadID == EspecialidadID) && m.locales.Any(l => l.LocalID == LocalID))
                .ToList();
            return medicos;
        }

        public Consulta obtenerConsulta(long ConsultaID)
        {
            var query = from c in db.consultas.Include("especialidad")
                        .Include("medico")
                        .Include("local")
                        .Include("pacientes")
                        .Include("pacientesespera")
                        where c.ConsultaID == ConsultaID
                        select c;
            Consulta con = query.Single();
            con.pacientesespera = con.pacientesespera.Take(3).ToList();
            return con;
        }

        public Local obtenerLocal(long LocalID)
        {
            var query = from l in db.locales
                        where l.LocalID == LocalID
                        select l;

            Local loc = query.Single();
            return loc;

        }

        public Especialidad obtenerEspecialidad(long EspecialidadID)
        {
            var query = from e in db.especialidades
                        where e.EspecialidadID == EspecialidadID
                        select e;

            Especialidad esp = query.Single();
            return esp;

        }

        public Medico obtenerMedico(string FuncionarioID)
        {
            var query = db.funcionarios
                        .OfType<Medico>().Single(x => x.FuncionarioID == FuncionarioID);

            Medico med = query;
            return med;

        }

        public ICollection<Paciente> listarPacientesNotInConsulta(long ConsultaID)
        {
         
            if (!db.consultas.Any(c => c.ConsultaID == ConsultaID))
                throw new Exception("No existe consulta");
            else
            {
                //var result = db.pacientes.Where(p => (!db.consultasagendadas.Any(c => (c.ConsultaID == ConsultaID) && (c.PacienteID.Equals(p.PacienteID))))
                //    && (!db.pacienteespera.Any(c => (c.ConsultaID == ConsultaID) && (c.PacienteID.Equals(p.PacienteID)))));

                var result = (from p in db.pacientes
                              where (!db.consultasagendadas.Any(c => (c.ConsultaID == ConsultaID) && (c.PacienteID == p.PacienteID)) &&
                              !db.pacienteespera.Any(c => (c.ConsultaID == ConsultaID) && (c.PacienteID == p.PacienteID)))
                             select p).ToList();

               

                return result;
            }
        }

    }
}
