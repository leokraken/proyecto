using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SAREM.Web.Controllers
{
    

    public class MedicoRefController : Controller
    {
        // GET: MedicoRef

        private FabricaSAREM fabrica;
        private string paciente;
        private string tenant;
       
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["usuario"] != null)
            {
                Debug.WriteLine("USUARIO NO ES NULL...");
                paciente = (string)filterContext.HttpContext.Session["usuario"];
                tenant = (string)filterContext.HttpContext.Session["tenant"];
                fabrica = new FabricaSAREM(tenant);
            }
            else
            {
                Debug.WriteLine("USUARIO NULL...");
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "GetLogOff", controller = "Account" }));
            }
        }
        
        #region DataTypes
        public class PacienteJson
        {
            public string PacienteID { get; set; }
            public string nombre { get; set; }
            public string celular { get; set; }
            public string telefono { get; set; }
            public string sexo { get; set; }
            public string fechaSolicitud { get; set; }
            public string fechaAprobacion { get; set; }
        }

        public class MedicoJson
        {
            public string MedicoID { get; set; }
            public string Nombre { get; set; }

        }

        public class LocalJson
        {
            public string Nombre { get; set; }
            public string Direccion { get; set; }

        }

        public class EstadoReferenciasJson
        {
            public string MedicoID { get; set; }
            public string Nombre { get; set; }
            public string Estado { get; set; }
            public string fechaSolicitud { get; set; }
            public string fechaConfirmacion { get; set; }
            
        }
        #endregion

        #region Tecnico
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerSolicitudes()
        {
            return View("VerSolicitudRefMedico");
        }

        public ActionResult VerSolicitudesAceptadas()
        {
            return View();
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


        [HttpGet]
        public JsonResult GetPacientesAprobados(string idM)
        {
            try
            {
                // TODO: Add delete logic here

                var pacientes = fabrica.ireferencias.obtenerPacientesReferenciadosMedico(idM);
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
                   
                   
                    DateTime fConf = r.fecha_confirmacion ?? DateTime.UtcNow;
                    runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                                fConf,
                                DateTimeKind.Utc);
                    DateTime localVersionFApr = runtimeKnowsThisIsUtc.ToLocalTime();
                    pjs.fechaAprobacion = localVersionFApr.ToString(format);
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

        #endregion

        #region Paciente

        [HttpGet]
        public ActionResult SeleccionarMedRef()
        {
            var model = new SAREM.Web.Models.MedicoReferencia
            {
                especialidad = fabrica.iespecialidades.listarEspecialidades(),

            };


            return View(model);
        }

       
        [HttpGet]
        public JsonResult GetMedico(string idM)
        {
            try
            {
                var medico = fabrica.imedicos.obtenerMedico(idM);
                MedicoJson m = new MedicoJson();
                m.MedicoID = medico.FuncionarioID;
                m.Nombre = medico.nombre;
                List<MedicoJson> mjs = new List<MedicoJson>();
                mjs.Add(m);
                return Json(mjs, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { success = false });
            }
        
        }
       
        [HttpGet]
        public JsonResult GetMedicoLocales(string idM)
        {
            try
            {
                var locales = fabrica.ilocales.listarLocalesMedico(idM);
             
                List<LocalJson> listjs = new List<LocalJson>();

                foreach (Local l in locales)
                {
                    LocalJson ljs = new LocalJson();
                    ljs.Nombre = l.nombre;
                    ljs.Direccion = l.calle + " " + l.numero;

                    listjs.Add(ljs);
                }

                return Json(listjs, JsonRequestBehavior.AllowGet);

            }
            catch(Exception e)
            {
                return Json(new { success = false });
            }
        
        }


        [HttpPost]
        public JsonResult SolicitarReferencia(string idM)
        {
            try
            {

                fabrica.ireferencias.agregarReferencia(paciente, idM);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }
       

        [HttpGet]
        public JsonResult GetEstadoReferencias()
        {
            try
            {
                var referencia = fabrica.ireferencias.obtenerReferencia(paciente);

                List<EstadoReferenciasJson> listjs = new List<EstadoReferenciasJson>();

                
                EstadoReferenciasJson e = new EstadoReferenciasJson();

                e.MedicoID = referencia.FuncionarioID;
                e.Nombre = referencia.medico.nombre;
                if (referencia.pendiente)
                {
                    e.Estado = "Pendiente";
                }
                else
                {
                    e.Estado = "Finalizado";
                }

                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        referencia.fecha_solicitud,
                            DateTimeKind.Utc);
                DateTime localVersionFSol = runtimeKnowsThisIsUtc.ToLocalTime();
                e.fechaSolicitud = localVersionFSol.ToString(format);

                if (referencia.fecha_confirmacion != null) { 
                
                    DateTime fConf = referencia.fecha_confirmacion ?? DateTime.UtcNow;
                    runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                            fConf,
                            DateTimeKind.Utc);
                    DateTime localVersionFApr = runtimeKnowsThisIsUtc.ToLocalTime();
                    e.fechaConfirmacion = localVersionFApr.ToString(format);

                }
                
                listjs.Add(e);
                

                return Json(listjs, JsonRequestBehavior.AllowGet);

            }
            catch(Exception e)
            {
                return Json(new { success = false });
            }
        
        }

        [HttpGet]
        public JsonResult ChequearSolicitud()
        {
            try
            {
                Boolean existeS = fabrica.ireferencias.chequearExistenciaSolicitud(paciente);
                return Json(new { existe = existeS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult CancelarReferencia(string idM)
        {
            try
            {

                fabrica.ireferencias.denegarReferencia(paciente, idM);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }

       

        public JsonResult GetMedicosTenant()
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Funcionario m in fabrica.imedicos.listarMedicos())
            {

                medicos.Add(new SelectListItem { Text = m.nombre, Value = m.FuncionarioID.ToString() });
            }
            return Json(new SelectList(medicos, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetMedicosEspecialidad(string idEspecialidad)
        {
            List<MedicoJson> mjs = new List<MedicoJson>();
            foreach (Funcionario m in fabrica.imedicos.listarMedicosEspecialidad(Convert.ToInt64(idEspecialidad)))
            {
                MedicoJson mj = new MedicoJson();
                mj.MedicoID = m.FuncionarioID;
                mj.Nombre = m.nombre;
           
                mjs.Add(mj);
            }

            return Json(mjs, JsonRequestBehavior.AllowGet);

        }


        #endregion
    }
}