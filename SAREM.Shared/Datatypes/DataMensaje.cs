using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Datatypes
{
    public class DataMensaje
    {
        public string medio { get; set; }
        public string asunto { get; set; }
        public string destinatario { get; set; }
        public string mensaje { get; set; }
        public DateTime fecha_envio { get; set; }
    }
}
