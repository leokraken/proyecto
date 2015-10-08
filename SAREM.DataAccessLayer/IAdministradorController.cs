using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IAdministradorController
    {
        ICollection<string> getSchemas();
        void createSchema(string schema);
        void dropSchema(string schema);
    }
}
