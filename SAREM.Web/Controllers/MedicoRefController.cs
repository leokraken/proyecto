using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAREM.Web.Controllers
{
    public class MedicoRefController : Controller
    {
        // GET: MedicoRef

        private FabricaSAREM fabrica = new FabricaSAREM("test");

        public class PacienteJson
        {
            public string PacienteID { get; set; }
            public string nombre { get; set; }
            public string celular { get; set; }
            public string telefono { get; set; }
            public string sexo { get; set; }
            public string fechaSolicitud { get; set; }
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerSolicitudes()
        {
            return View("VerSolicitudRefMedico");
        }

        [HttpGet]
        public JsonResult GetMedicos()
        {
            
            var medicos = fabrica.iagenda.listarFuncionarios();
            var ms =
                    from p in medicos
                    select new { MedicoID = p.FuncionarioID};

            return Json(ms, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPacientes(string idM)
        {
            try
            {
                // TODO: Add delete logic here

                var pacientes = fabrica.ireferencias.obtenerReferenciasPendientesMedico(idM);
                List<PacienteJson> pacientesjs = new List<PacienteJson>();
                foreach (Referencia r in pacientes)
                {
                    PacienteJson pjs = new PacienteJson();
                    pjs.PacienteID = r.paciente.PacienteID;
                    pjs.sexo = r.paciente.sexo.ToString();
                    pjs.nombre = r.paciente.nombre;
                    pjs.telefono = r.paciente.telefono;
                    pjs.celular = r.paciente.celular;
                    String format = "dd/MM/yyyy HH:mm";
                    DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                            r.fecha_solicitud,
                                DateTimeKind.Utc);
                    DateTime localVersionFSol = runtimeKnowsThisIsUtc.ToLocalTime();
                    pjs.fechaSolicitud = localVersionFSol.ToString(format);
                     
                    pacientesjs.Add(pjs);

                }
                return Json(pacientesjs, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false });
            }
           
        }

        [HttpPost]
        public JsonResult AprobarReferencia(string idM, string idP)
        {


            try
            {

                fabrica.ireferencias.finalizarReferencia(idP, idM);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }

        [HttpPost]
        public JsonResult DenegarReferencia(string idM, string idP)
        {
            try
            {

                fabrica.ireferencias.denegarReferencia(idP, idM);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }

        [HttpPost]
        public JsonResult AprobarAllRef(string idM)
        {
            try
            {
                var refs = fabrica.ireferencias.obtenerReferenciasPendientesMedico(idM);
                foreach (Referencia r in refs)
                {
                    fabrica.ireferencias.finalizarReferencia(r.PacienteID, idM);
                }
               
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }

        [HttpPost]
        public JsonResult DenegarAllRef(string idM)
        {
            try
            {
                var refs = fabrica.ireferencias.obtenerReferenciasPendientesMedico(idM);
                foreach (Referencia r in refs)
                {
                    fabrica.ireferencias.denegarReferencia(r.PacienteID,idM);
                }

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }
    }
}