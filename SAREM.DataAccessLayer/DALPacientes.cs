using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAREM.DataAccessLayer
{
    public class DALPacientes : IDALPacientes
    {
        private SARMContext db = null;

        public DALPacientes(string tenant)
        {
            db = SARMContext.getTenant(tenant);
        }
        public Paciente obtenerPaciente(string CI)
        {
            return db.pacientes.Find(CI);
        }

        public void altaPaciente(Paciente paciente)
        {
            if (!db.pacientes.Any(o => o.PacienteID==paciente.PacienteID))
            {
                throw new Exception("Paciente no existe");
            }
            else
            {
                db.pacientes.Add(paciente);
                db.SaveChanges();
            }
        }
        
        public void modificarPaciente(Paciente paciente)
        {
            if (!db.pacientes.Any(o => o.PacienteID == paciente.PacienteID))
            {
                throw new Exception("No existe paciente");
            }
            else
            {
                var p = db.pacientes.Find(paciente.PacienteID);
                p.nacionalidad = paciente.nacionalidad;
                p.celular = paciente.celular;
                p.direccion = paciente.direccion;
                p.FN = paciente.FN;
                p.nombre = paciente.nombre;
                p.sansion = paciente.sansion;
                p.sexo = paciente.sexo;
                p.telefono = paciente.telefono;
                db.SaveChanges();
            }
        }

        public ICollection<Paciente> listarPacientes()
        {
            return db.pacientes.ToList();
        }
        public void sancionarPaciente(string CI)
        {
            if (!db.pacientes.Any(p => p.PacienteID == CI))
                throw new Exception("No existe paciente");
            else
            {
                var p = db.pacientes.Find(CI);
                p.sansion = true;
                db.SaveChanges();
            }

        }
        public void eliminarPaciente(string CI)
        {
            if (!db.pacientes.Any(p => p.PacienteID == CI))
                throw new Exception("No existe paciente");
            else
            {
                var p = db.pacientes.Find(CI);
                db.pacientes.Remove(p);
                db.SaveChanges();
            }
        }
    }
}
