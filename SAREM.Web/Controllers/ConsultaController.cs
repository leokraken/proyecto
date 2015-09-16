using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
//using SAREM.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SAREM.Web.Controllers
{
    public class ConsultaController : Controller
    {
        private IDALAgenda agenda = new DALAgenda("test");
        private IDALPacientes pacienteDal = new DALPacientes("test");

        private static string[] formats = new string[]
        {
            "dd/MM/yyyy H:mm",
            "dd/MM/yyyy HH:mm"    
        };

        public static DateTime ParseDate(string input)
        {
            return DateTime.ParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public class GetConsultasJSON
        {
            public List<ConsultaJSON> records { get; set; }
        }

        public class ConsultaJSON
        {
            public string idC { get; set; }
            public string origen { get; set; }
            public string especialidad { get; set; }
            public string medico { get; set; }
            public String fechaInicio { get; set; }
            public String fechaFin { get; set; }
        }

        public class LocalJson
        {
            public string LocalID { get; set; }
            public string nombre { get; set; }
            public Boolean sel { get; set; }
        }

        public class EspecialidadJson
        {
            public string EspecialidadID { get; set; }
            public string descripcion { get; set; }
            public Boolean sel { get; set; }
        }

        public class MedicoJson
        {
            public string FuncionarioID { get; set; }
            public string nombre { get; set; }
            public Boolean sel { get; set; }
        }

        public class GetLocalEspMedJson
        {
            public List<LocalJson> locales { get; set; }
            public List<MedicoJson> medicos { get; set; }
            public List<EspecialidadJson> especialidades { get; set; }
        }


        public class Pacientes
        {
            public List<PacienteJson> records { get; set; }
        }

        public class PacienteJson
        {
            public string PacienteID { get; set; }
            public string nombre { get; set; }
            public string celular { get; set; }
            public string telefono { get; set; }
            public string sexo { get; set; }
            public string fechaRegistro { get; set; }
            public string numero { get; set; }
        }

        // GET: Consulta
        public ActionResult Index()
        {
           
            return View();
        }

        // GET: Consulta/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Consulta/Create
        [HttpGet]
        public ActionResult Create()
        {
            var model = new SAREM.Web.Models.Consulta
            {
               local = agenda.listarLocales(),
               
               //especialidades = agenda.listarEspecialidades(),
               //funcionarios = agenda.listarFuncionarios()
            };


            return View(model);
        }

        // POST: Consulta/Create
        [HttpPost]
        public JsonResult Create(SAREM.Web.Models.Consulta consulta)
        {

            if (ModelState.IsValid)
            {
              
                SAREM.Shared.Entities.Consulta c = new SAREM.Shared.Entities.Consulta();
                c.LocalID = Convert.ToInt64(consulta.localID);
                c.local = agenda.obtenerLocal(c.LocalID);
              
                c.EspecialidadID = Convert.ToInt64(consulta.especialidadID);
                c.especialidad = agenda.obtenerEspecialidad(c.EspecialidadID);
                c.FuncionarioID = consulta.medID;
                c.medico = agenda.obtenerMedico(c.FuncionarioID);

               
                c.fecha_fin = ParseDate(consulta.fecha_fin).ToUniversalTime();
                c.fecha_inicio = ParseDate(consulta.fecha_inicio).ToUniversalTime();
                

               
                agenda.agregarConsulta(c);
              
                return Json(new { success = true});
            }

            return Json(new { success = false});

            
        }



        public JsonResult GetEspecialidades(string idLocalidad)
        {
            List<SelectListItem> especialidades = new List<SelectListItem>();
            foreach (Especialidad e in agenda.listarEspecialidadesLocal(Convert.ToInt64(idLocalidad)))
            {

                especialidades.Add(new SelectListItem { Text = e.descripcion, Value = e.EspecialidadID.ToString() });
            }
            return Json(new SelectList(especialidades, "Value", "Text"), JsonRequestBehavior.AllowGet);
          
        }


        public JsonResult GetMedicos(string idEspecialidad, string idLocalidad)
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Funcionario m in agenda.listarMedicosEspecialidadLocal(Convert.ToInt64(idLocalidad), Convert.ToInt64(idEspecialidad)))
            {

                medicos.Add(new SelectListItem { Text = m.nombre, Value = m.FuncionarioID.ToString() });
            }
            return Json(new SelectList(medicos, "Value", "Text"));
        }


        public ActionResult VerConsultas()
        {
            return View("VerConsultasNew");
        }

       


        public JsonResult GetConsultas()
        {
            var consultas = agenda.listarConsultas();
            List<ConsultaJSON> lista = new List<ConsultaJSON>();
           
            foreach (Consulta c in consultas)
            {
                ConsultaJSON cjson = new ConsultaJSON();

                cjson.idC = c.ConsultaID.ToString();
                cjson.origen = c.local.nombre;
                cjson.especialidad = c.especialidad.descripcion;
                cjson.medico = c.medico.nombre;

                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        c.fecha_inicio,
                            DateTimeKind.Utc);
                DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                cjson.fechaInicio = localVersionFIni.ToString(format);

                runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                       c.fecha_fin,
                           DateTimeKind.Utc);
                localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();

                cjson.fechaFin = localVersionFIni.ToString(format);
            
                lista.Add(cjson);
            }

            var aux = new GetConsultasJSON
            {

                records = lista
            };
          

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: Consulta/Edit/5
        public JsonResult Edit(string idC)
        {
            long idL = Convert.ToInt64(idC);
            GetLocalEspMedJson obj = new GetLocalEspMedJson();
            List<LocalJson> locales = new List<LocalJson>();
            Consulta c = agenda.obtenerConsulta(idL);
            foreach (Local l in agenda.listarLocales()) {

                LocalJson lj = new LocalJson();
                lj.LocalID = l.LocalID.ToString();
                lj.nombre = l.nombre;
                if (l.LocalID == c.local.LocalID)
                {
                    lj.sel = true;
                } else {
                    lj.sel = false;
                }

                locales.Add(lj);
            }

            obj.locales = locales;

            List<EspecialidadJson> esps = new List<EspecialidadJson>();
            var espsLocal = agenda.listarEspecialidadesLocal(c.LocalID);
            foreach (Especialidad e in espsLocal) {

                EspecialidadJson ejson = new EspecialidadJson();
                ejson.EspecialidadID = e.EspecialidadID.ToString();
                ejson.descripcion = e.descripcion;

                if (e.EspecialidadID == c.EspecialidadID)
                {
                    ejson.sel = true;

                } else {

                    ejson.sel = false;
                }

                esps.Add(ejson);
            }

            obj.especialidades = esps;


            List<MedicoJson> meds = new List<MedicoJson>();
            var medsespsLocal = agenda.listarMedicosEspecialidadLocal(c.LocalID, c.EspecialidadID);

            foreach (Medico m in medsespsLocal)
            {
                MedicoJson mjson = new MedicoJson();
                mjson.FuncionarioID = m.FuncionarioID;
                mjson.nombre = m.nombre;
                if (m.FuncionarioID.Equals(c.FuncionarioID)) {

                   mjson.sel = true;

                } else {

                    mjson.sel = false;
                }

                meds.Add(mjson);
            }

            obj.medicos = meds;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        // POST: Consulta/Edit/5
        [HttpPost]
        public JsonResult Edit(SAREM.Web.Models.Consulta consulta)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    SAREM.Shared.Entities.Consulta c = new SAREM.Shared.Entities.Consulta();
                    c.LocalID = Convert.ToInt64(consulta.localID);
                    c.local = agenda.obtenerLocal(c.LocalID);
                    c.ConsultaID = Convert.ToInt64(consulta.consultaID);
                    c.EspecialidadID = Convert.ToInt64(consulta.especialidadID);
                    c.especialidad = agenda.obtenerEspecialidad(c.EspecialidadID);
                    c.FuncionarioID = consulta.medID;
                    c.medico = agenda.obtenerMedico(c.FuncionarioID);


                    c.fecha_fin = ParseDate(consulta.fecha_fin).ToUniversalTime();
                    c.fecha_inicio = ParseDate(consulta.fecha_inicio).ToUniversalTime();



                    agenda.modificarConsulta(c);

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            catch
            {
                return Json(new { success = false });
            }
        }

       

        // POST: Consulta/Delete/5
        [HttpPost]
        public JsonResult Delete(string idC)
        {
            try
            {
                // TODO: Add delete logic here

                long idCC = Convert.ToInt64(idC);
                agenda.eliminarConsulta(idCC);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

       
        [HttpGet]
        public ActionResult VerPacientes(string idC)
        {
            long idL = Convert.ToInt64(idC);
            Consulta c = agenda.obtenerConsulta(idL);
            String format = "dd/MM/yyyy HH:mm";
            DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        c.fecha_inicio,
                            DateTimeKind.Utc);
            DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
            var fI = localVersionFIni.ToString(format);

            runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                     c.fecha_fin,
                         DateTimeKind.Utc);
            localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
            var fFin = localVersionFIni.ToString(format);
            
            var model = new SAREM.Web.Models.Consulta
            {
                consultaID = idC,
                fecha_inicio = fI,
                fecha_fin = fFin,
                descEspecialidad = c.especialidad.descripcion,
                localDesc = c.local.nombre,
                medDesc = c.medico.nombre
            };


            return View("VerConsultaPaciente",model);
        }

        [HttpGet]
        public JsonResult GetPacientes(string idC)
        {
            long idL = Convert.ToInt64(idC);
            Pacientes obj = new Pacientes();
            List<PacienteJson> pacientes = new List<PacienteJson>();

            var pacientesConsulta = agenda.obtenerConsulta(idL).pacientes;
            var pacientesOrdered = agenda.obtenerPacientesConsulta(idL);
            int nro = 1;
            foreach (Paciente p in pacientesOrdered)
            {

                var pC = pacientesConsulta.First(x =>( x.PacienteID == p.PacienteID &&  x.ConsultaID == idL));

                PacienteJson pj = new PacienteJson();
                Paciente pente = pacienteDal.obtenerPaciente(p.PacienteID);
                pj.PacienteID = p.PacienteID;
                pj.nombre = pente.nombre;
                //pj.celular = pente.celular;
                //pj.telefono = pente.telefono;
                //Cambiar por valores reales luego
                pj.celular = "098258908";
                pj.telefono = "29014567";
                pj.sexo = pente.sexo.ToString();
                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        pC.fecharegistro,
                            DateTimeKind.Utc);
                DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                pj.fechaRegistro = localVersionFIni.ToString(format);
                pj.numero = nro.ToString();
                nro++;
                pacientes.Add(pj);
               
            }

            obj.records = pacientes;
            return Json(obj.records, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPacientesNotInConsulta(string idC)
        {
            long idL = Convert.ToInt64(idC);
            var pacientesNotInConsulta = agenda.listarPacientesNotInConsulta(idL);
            var pacientes =
                    from p in pacientesNotInConsulta
                    select new { PacienteID = p.PacienteID};

            return Json(pacientes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPacientesEspera(string idC)
        {
            long idL = Convert.ToInt64(idC);
            Pacientes obj = new Pacientes();
            List<PacienteJson> pacientes = new List<PacienteJson>();
           
            var pacientesConsulta = agenda.obtenerConsulta(idL).pacientesespera;
            var pacientesEspOrdered = agenda.obtenerPacientesConsultaEspera(idL);
            int nro = 1;
            foreach (Paciente p in pacientesEspOrdered)
            {
                var pC = pacientesConsulta.First(x => (x.PacienteID == p.PacienteID && x.ConsultaID == idL));
                PacienteJson pj = new PacienteJson();
                Paciente pente = pacienteDal.obtenerPaciente(p.PacienteID);
                pj.PacienteID = p.PacienteID;
                pj.nombre = pente.nombre;
                //pj.celular = pente.celular;
                //pj.telefono = pente.telefono;
                //Cambiar por valores reales luego
                pj.celular = "098258908";
                pj.telefono = "29014567";
                pj.sexo = pente.sexo.ToString();
                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        pC.fecha,
                            DateTimeKind.Utc);
                DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                pj.fechaRegistro = localVersionFIni.ToString(format);
                pj.numero = nro.ToString();
                nro++;
                pacientes.Add(pj);

            }

            obj.records = pacientes;
            return Json(obj.records, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddPacienteConsulta(string idC, string idP)
        {

         
            try
            {
                long idCC = Convert.ToInt64(idC);
                agenda.agregarConsultaPaciente(idP, idCC);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

          
        }

        [HttpPost]
        public JsonResult AddPacienteConsultaEspera(string idC, string idP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                agenda.agregarConsultaPacienteEspera(idP, idCC);
               
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }


        [HttpPost]
        public JsonResult CancelPacienteConsulta(string idC, string idP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                agenda.cancelarConsultaPaciente(idP, idCC);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }


        [HttpPost]
        public JsonResult EliminarPacienteConsultaLE(string idC, string idP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                agenda.eliminarPacienteConsultaLE(idP, idCC);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }


        [HttpPost]
        public JsonResult MoverPacientesConsultaLE(string idC, List<string> idsP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);

                agenda.moverPacientesLEConsulta(idsP, idCC);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }
 
    }
}
