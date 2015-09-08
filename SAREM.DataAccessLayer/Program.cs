using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARM.DataAccessLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var db = new SAREMAdminContext())
            {
                string schema = "test";
                Console.WriteLine("Delete schema: " + schema);
                db.dropSchema(schema);
                Console.WriteLine("Create schema: "+ schema);
                SARMContext.createTenant(schema);

            }
            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
    }
}
