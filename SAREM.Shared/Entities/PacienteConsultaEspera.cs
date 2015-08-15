using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class PacienteConsultaEspera
    {
        [Key, Column(Order=1)]
        public string PacienteID { get; set; }
        [Key, Column(Order = 2)]
        public long ConsultaID { get; set; }
        public DateTime fecha { get; set; }

        public virtual Paciente paciente { get; set; }
        public virtual Consulta consulta { get; set; }
    }
}
