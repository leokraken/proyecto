using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAREM.Shared.Entities;

namespace SAREM.Shared.Datatypes
{
    public class DataConsultaPaciente
    {
        public Consulta consulta { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public DateTime? turno { get; set; }
        public Boolean espera { get; set; }
    }
}