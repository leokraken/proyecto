using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARM.DataAccessLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            SARMContext.createTenant("Prueba");
            using(var db = SARMContext.getTenant("Prueba"))
            {

            }
            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
    }
}
