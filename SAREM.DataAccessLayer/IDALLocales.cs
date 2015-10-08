using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALLocales
    {
        ICollection<Local> listarLocales();
        Local obtenerLocal(long LocalID);
        ICollection<Local> listarLocales(long EspecialidadID);
        ICollection<MedicoLocal> listarLocalesMedico(string medicoID);

    }
}
