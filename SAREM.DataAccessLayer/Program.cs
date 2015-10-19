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
            DALAMQP amq = new DALAMQP("test");
            var m = new DataMensaje
            {
                medio = 1, //mail
                destinatario = "chebaysend@gmail.com",
                asunto = "RabbitMQ testing",
                mensaje = "Testing python send mail!",
                fecha_envio = DateTime.UtcNow.AddMinutes(1),
                inmediato=false
            };
            amq.sendToQueue(m);
        }

        void testConsultamail() {
            FabricaSAREM f = new FabricaSAREM("test");
            /*
            Consulta c = new Consulta
            {
                EspecialidadID = 1,
                fecha_inicio = DateTime.UtcNow.AddMinutes(2),
                fecha_fin = DateTime.UtcNow.AddMinutes(12),
                FuncionarioID = "17299995",
                LocalID = 1,
                numpacientes = 10,
                maxpacientesespera = 10
            };
            f.iagenda.agregarConsulta(c);
            long consultaid = 
             * */
           
        }

        static void Main(string[] args)
        {
            FabricaSAREM f = new FabricaSAREM("test");
            //f.iagenda.cancelarConsultaPaciente("1", 2);
            //f.iagenda.agregarConsultaPaciente("0", 1, 1, false);
            f.iagenda.agregarConsultaPaciente("50548305", 1, 1, false);
            //amq();
            Console.WriteLine("finish");
            Console.Read();
            //testOpenempi();
        }
    }
}
