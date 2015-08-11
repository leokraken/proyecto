using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALEvento
    {
        void suscribirPacienteEvento(long EventoID, string PacienteID, long ComunicacionID);

    }
}
