using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class Pais
    {
        public string PaisID { get; set; }
        public string nombre { get; set; }

        public ICollection<Paciente> pacientes { get; set; }
    }
}
