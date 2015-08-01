using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class Nacion
    {
        public string NacionID { get; set; }
        public string nombre { get; set; }

        public ICollection<Paciente> pacientes { get; set; }
    }
}
