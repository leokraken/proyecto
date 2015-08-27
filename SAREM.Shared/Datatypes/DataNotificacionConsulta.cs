using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Datatypes
{
    public class DataNotificacionConsulta
    {
        //obtengo el medio de comunicacion e implemento los protocolos sms, mail, ws
        public string PacienteID { get; set; }
        public string PacientePais { get; set; }
        public long EventoID { get; set; }
        public DateTime fecha { get; set; }
    }
}
