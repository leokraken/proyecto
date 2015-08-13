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
            //SARMContext.createTenant("testschema");
            using(var db = new SAREMAdminContext())
            {
                db.dropSchema("test");
                //db.dropSchema("testschema");
            }

            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
    }
}
