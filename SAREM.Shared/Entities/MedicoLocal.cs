using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class MedicoLocal
    {
        [Key, Column(Order=1)]
        public string FuncionarioID { get; set; }
        [Key, Column(Order = 2)]
        public long LocalID { get; set; }

        public virtual Medico medico { get; set; }
        public virtual Local local { get; set; }
    }
}
