using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class Mensaje
    {
        [Key]
        public string MensajeID { get; set; }
        public string PacienteID { get; set; }
        public long ConsultaID { get; set; }
        public string Body { get; set; }
        DateTime fecha_release { get; set; }

        public virtual Paciente paciente { get; set; }
        public virtual Consulta consulta { get; set; }
    }
}
