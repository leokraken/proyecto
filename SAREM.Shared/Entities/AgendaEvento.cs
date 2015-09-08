using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class AgendaEvento
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public long ConsultaID { get; set; }
        public long EventoID { get; set; }

        [ForeignKey("ConsultaID")]
        public virtual Consulta consulta { get; set; }
        [ForeignKey("EventoID")]
        public virtual EventoObligatorio evento { get; set; }
        public DateTime fechanotificacion { get; set; }
    }
}
