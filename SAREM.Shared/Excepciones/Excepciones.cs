using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Excepciones
{
    public class ExcepcionMaxPacientesEspera : Exception
    {
        public ExcepcionMaxPacientesEspera() { }
    }

    public class ExcepcionNoExisteConsulta : Exception
    {
        public ExcepcionNoExisteConsulta() { }
    }

    public class ExcepcionNoExistePaciente : Exception
    {
        public ExcepcionNoExistePaciente() { }
    }

    public class ExcepcionFueraDeFechaParteDiario : Exception
    {
        public ExcepcionFueraDeFechaParteDiario() { }
    }

}
