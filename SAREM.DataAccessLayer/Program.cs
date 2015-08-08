using SAREM.DataAccessLayer;
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
            //IDALAgenda idal = new DALAgenda("testschema");
            //var lista =idal.listarLocales();
            //Console.WriteLine(lista.Count);
            using(var db = new SAREMAdminContext())
            {
                db.dropSchema("conn");
            }

            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
    }
}
