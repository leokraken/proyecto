using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void agregarConsultaPaciente(string PacienteID, long ConsultaID, Boolean fueraLista)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Paciente paciente = db.pacientes.Find(PacienteID);
                if (paciente != null)
                {
                    Consulta consulta = db.consultas.Find(ConsultaID);
                    int numpacientes = db.consultas.Include("pacientes")
                                         .Where(c => c.ConsultaID == ConsultaID)
                                         .Single().pacientes.Count;


                    if (consulta != null)
                    {
                        //int time = ((DateTime)consulta.fecha_inicio - DateTime.UtcNow).Hours;

                        if (numpacientes < MAX_PACIENTES)
                        {

                            var conscans = db.consultascanceladas.SingleOrDefault(x => (x.ConsultaID == ConsultaID) && (x.PacienteID == PacienteID));
                            if (conscans != null)
                            {
                                db.consultascanceladas.Remove(conscans);
                                db.SaveChanges();
                            }

                            PacienteConsultaAgenda pca = new PacienteConsultaAgenda { ConsultaID = ConsultaID, PacienteID = PacienteID, fecharegistro = DateTime.UtcNow , fueralista = fueraLista};
                            db.consultasagendadas.Add(pca);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Ya existen 10 Pacientes en Consulta::" + numpacientes);
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

                        if (numpacientes < MAX_PACIENTES_ESPERA)
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
                    }
                    else
                    {
                        throw new Exception("No existe consulta");
                    }
                }
                else
                {
                    throw new Exception("No existe paciente");
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
                if (db.consultas.Any(c => c.ConsultaID == ConsultaID)
                && db.pacientes.Any(p => p.PacienteID == PacienteID))
                {
                    var conscans = db.consultascanceladas.SingleOrDefault(x => (x.ConsultaID == ConsultaID) && (x.PacienteID == PacienteID));
                    if (conscans != null)
                    {
                        db.consultascanceladas.Remove(conscans);
                        db.SaveChanges();
                    }

                    var cancelar = new PacienteConsultaCancelar
                    {
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
            }
        }

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
                    //c.local = db.locales.Find(consulta.LocalID);
                    c.EspecialidadID = consulta.EspecialidadID;
                    // c.especialidad = this.obtenerEspecialidad(consulta.EspecialidadID);
                    c.fecha_fin = consulta.fecha_fin;
                    c.fecha_inicio = consulta.fecha_inicio;
                    c.FuncionarioID = consulta.FuncionarioID;
                    //c.medico = this.obtenerMedico(consulta.FuncionarioID);
                    db.SaveChanges();
                }
            }
        }

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
                else
                {
                    var q = from c in db.consultasagendadas.Include("consulta")
                            where c.PacienteID == PacienteID
                            select c.consulta;
                    return q.ToList();
                }
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

        public ICollection<Especialidad> listarEspecialidades()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                return db.especialidades.ToList();
            }
        }

        public ICollection<Funcionario> listarFuncionarios()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                return db.funcionarios.ToList();
            }
        }

        public ICollection<Especialidad> listarEspecialidadesLocal(long LocalID)
        {
            using (var db = SARMContext.getTenant(tenant))
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
        }

        public ICollection<Medico> listarMedicosEspecialidadLocal(long LocalID, long EspecialidadID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var medicos = db.funcionarios
                .OfType<Medico>()
                .Where(m => m.especialidades.Any(e => e.EspecialidadID == EspecialidadID) && m.locales.Any(l => l.LocalID == LocalID))
                .ToList();
                return medicos;
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
                con.pacientesespera = con.pacientesespera.Take(3).ToList();
                return con;
            }
        }

        public Especialidad obtenerEspecialidad(long EspecialidadID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from e in db.especialidades
                            where e.EspecialidadID == EspecialidadID
                            select e;

                Especialidad esp = query.Single();
                return esp;
            }
        }

        public Medico obtenerMedico(string FuncionarioID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = db.funcionarios
                            .OfType<Medico>().Single(x => x.FuncionarioID == FuncionarioID);

                Medico med = query;
                return med;
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
                var pacientesOrdered = db.consultasagendadas.Include("paciente").Include("consulta").Where(x => x.ConsultaID == ConsultaID)
                    .OrderByDescending(x => (x.paciente.FuncionarioID != null && x.paciente.FuncionarioID == x.consulta.FuncionarioID))
                    .ThenBy(x => x.fecharegistro).Select(p => p.paciente).ToList();
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


        // Por definicion el parte diario corresponde a la consulta de un medico
        // dada una fecha.
        public ICollection<Consulta> obtenerParteDiario(string MedicoID, DateTime fecha)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                if (fecha.Date <= DateTime.UtcNow.Date)
                {
                    var query = from c in db.consultas.Include("especialidad")
                            .Include("medico")
                            .Include("local").Include("pacientes.paciente")
                                where c.FuncionarioID == MedicoID && fecha.Date == fecha.Date
                                select c;
                    return query.ToList();
                }
                else
                {
                    throw new Exception("El parte diario para dicha fecha no existe");
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
                PacienteConsultaAgenda pca = query.Single();
                pca.diagnostico = diagnostico;
                pca.ausencia = ausencia;
                db.Entry(pca).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}
