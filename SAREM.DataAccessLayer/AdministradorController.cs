using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class AdministradorController : IAdministradorController
    {
        public ICollection<string> getSchemas()
        {
            using (var db = new SAREMAdminContext())
            {
                return db.getSchemas();
            }
        }

        public void createSchema(string schema)
        {
            SARMContext.createTenant(schema);
        }

        public void dropSchema(string schema)
        {
            using (var db = new SAREMAdminContext())
            {
                db.dropSchema(schema);
            }           
        }
    }
}
