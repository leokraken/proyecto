using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALMedicos : IDALMedicos
    {
        private string tenant;
        public DALMedicos(string tenant)
        {
            this.tenant = tenant;
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

        public ICollection<Medico> listarMedicosEspecialidad(long EspecialidadID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var medicos = db.funcionarios
                .OfType<Medico>()
                .Where(m => m.especialidades.Any(e => e.EspecialidadID == EspecialidadID))
                .ToList();
                return medicos;
            }
        }

        public ICollection<Medico> listarMedicosLocal(long LocalID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var medicos = db.funcionarios
                .OfType<Medico>()
                .Where(m => m.locales.Any(l => l.LocalID == LocalID))
                .ToList();
                return medicos;
            }
        }

        public ICollection<Medico> listarMedicos()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var medicos = db.funcionarios
                .OfType<Medico>()
                .ToList();
                return medicos;
            }
        }
    }
}
