using SAREM.DataAccessLayer;
using SAREM.Shared.Datatypes;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SARM.DataAccessLayer
{
    class Program
    {
        public static void testOpenempi()
        {
            FabricaSAREM f = new FabricaSAREM("test");
            Console.WriteLine("Openempi test.");

            DataPaciente p = f.iopenempi.obtenerPaciente("UY", "5432");
            if (p != null){
                Console.WriteLine(p.paciente.mail);
                Console.WriteLine(p.mutualista);
            }
            else
                Console.WriteLine("Persona no encontrada...");

            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }

        static void amq()
        {
            DALAMQP amq = new DALAMQP();
            var m = new DataMensaje
            {
                medio = "mail",
                destinatario = "tremendomail@mail.com",
                asunto = "test",
                mensaje = "hello world!",
                fecha_envio = DateTime.Now.AddDays(2)
            };

        }

        static void Main(string[] args)
        {
            FabricaSAREM f = new FabricaSAREM("test");
            f.iagenda.cancelarConsultaPaciente("1", 2);

            Console.Read();
            //testOpenempi();
        }
    }
}
