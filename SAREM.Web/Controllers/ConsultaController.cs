﻿using OfficeOpenXml;
using SAREM.DataAccessLayer;
using SAREM.Shared.Datatypes;
using SAREM.Shared.Entities;
using SAREM.Shared.Excepciones;
using SAREM.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SAREM.Web.Controllers
{
    public class FilterInit : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["usuario"] == null)
            {
                Debug.WriteLine("USUARIO ES NULL");
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "GetLogOff", controller = "Account" }));
            }else
            {
                Debug.WriteLine("USUARIO NO ES NULL...");
            }
        }
    }
   
    public class ConsultaController : Controller
    {
        private string paciente;
        private string tenant;
        private FabricaSAREM fabrica;// = new FabricaSAREM("test");

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
        private static string[] formats = new string[]
        {
            "dd/MM/yyyy H:mm",
            "dd/MM/yyyy HH:mm", 
            "dd/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy"
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
            public string turno { get; set; }
            public string cantP { get; set; }
            public string cantPE { get; set; }
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
            public string fueradeLista { get; set; }
            public string ausencia { get; set; }
            public string turno { get; set; }
        }

        #endregion

        #region Tecnico

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
               local =  fabrica.ilocales.listarLocales(),
               
               //especialidades =  fabrica.iagenda.listarEspecialidades(),
               //funcionarios =  fabrica.iagenda.listarFuncionarios()
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
                //c.local =  fabrica.ilocales.obtenerLocal(c.LocalID);
              
                c.EspecialidadID = Convert.ToInt64(consulta.especialidadID);
                //c.especialidad =  fabrica.iagenda.obtenerEspecialidad(c.EspecialidadID);
                c.FuncionarioID = consulta.medID;
                //c.medico =  fabrica.iagenda.obtenerMedico(c.FuncionarioID);

               
                c.fecha_fin = ParseDate(consulta.fecha_fin).ToUniversalTime();
                c.fecha_inicio = ParseDate(consulta.fecha_inicio).ToUniversalTime();

                short cP;
                if (!short.TryParse(consulta.cantPacientes, out cP))
                {
                    cP = 0;
                }

                c.numpacientes = cP;

                short cPE;
                if (!short.TryParse(consulta.cantPacientesEspera, out cPE))
                {
                    cPE = 0;
                }

                c.maxpacientesespera = cPE;
               
                fabrica.iagenda.agregarConsulta(c);
              
                return Json(new { success = true});
            }

            return Json(new { success = false});

            
        }

        [HttpGet]
        public ActionResult CreateMasivo()
        {
            ConsultaMasiva cM = new ConsultaMasiva();
            cM.success = false;
            cM.warning = false;
            cM.danger = false;
            return View("CreateMasivo",cM);
        }


        
        [HttpGet]
        public ActionResult DownloadTemplate()
        {

            string fullPath = Path.Combine(Server.MapPath("~/Template"), "Template-Consulta.xlsx");
            return File(fullPath, "application/vnd.ms-excel", "Template-Consulta.xlsx");
          
           
        }

      
        public ActionResult Upload(FormCollection formCollection)

        {
            ConsultaMasiva cM = new ConsultaMasiva();
            Boolean okConsultas = true;
            cM.danger = false;
            cM.warning = false;
            cM.success = false;
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    //Creo las consultas
                    var consultaList = new List<SAREM.Shared.Entities.Consulta>();
                    try
                    {
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                var c = new SAREM.Shared.Entities.Consulta();
                                Boolean ok = true;

                                if(!(workSheet.Cells[rowIterator, 1].Value == null &&
                                    workSheet.Cells[rowIterator, 2].Value == null &&
                                    workSheet.Cells[rowIterator, 3].Value == null &&
                                    workSheet.Cells[rowIterator, 4].Value == null &&
                                    workSheet.Cells[rowIterator, 5].Value == null && workSheet.Cells[rowIterator, 6].Value == null && workSheet.Cells[rowIterator, 7].Value == null))
                                {
                                 for (int columnIterator = 1; (columnIterator <= 7 && ok); columnIterator++)
                                  {



                                    switch (columnIterator)
                                    {
                                        case 1:
                                            //ID LOCAL
                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString()))
                                                {
                                                
                                                    c.LocalID = Convert.ToInt64(workSheet.Cells[rowIterator, columnIterator].Value.ToString());
                                                   // c.local =  fabrica.ilocales.obtenerLocal(c.LocalID);

                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }

                                            break;
                                        case 2:
                                            //ID ESPECIALIDAD
                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString())) { 
                                                    c.EspecialidadID = Convert.ToInt64(workSheet.Cells[rowIterator, columnIterator].Value.ToString());
                                                   // c.especialidad =  fabrica.iagenda.obtenerEspecialidad(c.EspecialidadID);
                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }
                                            break;
                                        case 3:
                                            //ID MEDICO
                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString()))
                                                {
                                                    string nroDoc = workSheet.Cells[rowIterator, columnIterator].Value.ToString();

                                                    if (String.IsNullOrEmpty(nroDoc) || !Regex.IsMatch(nroDoc, @"^\d+$"))
                                                    {

                                                        ok = false;

                                                    }
                                                    else
                                                    {
                                                        c.FuncionarioID = nroDoc;
                                                    }
                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }
                                            break;
                                        case 4:
                                            //Fecha Inicio


                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString())) { 
 
                                                    c.fecha_inicio = DateTime.Parse(workSheet.Cells[rowIterator, columnIterator].Value.ToString()).ToUniversalTime();
                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }

                                            break;

                                        case 5:
                                            //Fecha Fin


                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString()))
                                                {
                                                    c.fecha_fin = DateTime.Parse(workSheet.Cells[rowIterator, columnIterator].Value.ToString()).ToUniversalTime();
                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }

                                            break;

                                        case 6:
                                            //Cantidad de pacientes en consulta
                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString()))
                                                {
                                                    c.numpacientes = Convert.ToInt16(workSheet.Cells[rowIterator, columnIterator].Value.ToString());
                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }

                                            break;
                                        case 7:
                                            //Cantidad de pacientes en lista de espera
                                            try
                                            {
                                                if (!String.IsNullOrEmpty(workSheet.Cells[rowIterator, columnIterator].Value.ToString()))
                                                {
                                                    c.maxpacientesespera = Convert.ToInt16(workSheet.Cells[rowIterator, columnIterator].Value.ToString());
                                                }
                                                else
                                                {
                                                    ok = false;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ok = false;
                                            }

                                            break;
                                    }

                                }
                               

                                if (ok)
                                {
                                     fabrica.iagenda.agregarConsulta(c);
                                } else
                                {
                                    okConsultas = ok && okConsultas;

                                }

                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {   
                        cM.danger = true;
                        cM.warning = false;
                        cM.success = false;
                        return View("CreateMasivo", cM);
                    }
                }
                else
                {
                    cM.danger = true;
                    cM.warning = false;
                    cM.success = false;
                    return View("CreateMasivo", cM);

                }
            }
           

            if (okConsultas)
            {
                cM.danger = false;
                cM.warning = false;
                cM.success = true;

            }
            else
            {
                cM.danger = false;
                cM.warning = true;
                cM.success = false;
            }

            return View("CreateMasivo", cM);
        }


        public JsonResult GetTurnosConsulta(string idC)
        {
            List<SelectListItem> turnos = new List<SelectListItem>();
            var ts = fabrica.iagenda.obtenerTurnosLibres(Convert.ToInt64(idC));
              
            foreach (PacienteConsultaAgenda c in ts)
            {

                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        (DateTime)c.turno,
                            DateTimeKind.Utc);
                DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();

                turnos.Add(new SelectListItem { Text = localVersionFIni.ToString(format), Value = c.ConsultaIDTurno.ToString() });
            }

            if (ts.Count == 0)
            {
                return Json(new { turnoVacio = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new SelectList(turnos, "Value", "Text"), JsonRequestBehavior.AllowGet);

            }
          

        }



        public JsonResult GetEspecialidades(string idLocalidad)
        {
            List<SelectListItem> especialidades = new List<SelectListItem>();
            foreach (Especialidad e in  fabrica.iespecialidades.listarEspecialidadesLocal(Convert.ToInt64(idLocalidad)))
            {

                especialidades.Add(new SelectListItem { Text = e.descripcion, Value = e.EspecialidadID.ToString() });
            }
            return Json(new SelectList(especialidades, "Value", "Text"), JsonRequestBehavior.AllowGet);
          
        }


        public JsonResult GetMedicos(string idEspecialidad, string idLocalidad)
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Funcionario m in  fabrica.imedicos.listarMedicosEspecialidadLocal(Convert.ToInt64(idLocalidad), Convert.ToInt64(idEspecialidad)))
            {

                medicos.Add(new SelectListItem { Text = m.nombre, Value = m.FuncionarioID.ToString() });
            }
            return Json(new SelectList(medicos, "Value", "Text"));
        }

        public JsonResult GetMedicosEspecialidad(string idEspecialidad)
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Funcionario m in fabrica.imedicos.listarMedicosEspecialidad(Convert.ToInt64(idEspecialidad)))
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
            var consultas =  fabrica.iagenda.listarConsultas();
            List<ConsultaJSON> lista = new List<ConsultaJSON>();
           
            foreach (SAREM.Shared.Entities.Consulta c in consultas)
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

                cjson.cantP = c.numpacientes.ToString();
                cjson.cantPE = c.maxpacientesespera.ToString();
            
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
            SAREM.Shared.Entities.Consulta c =  fabrica.iagenda.obtenerConsulta(idL);
            foreach (Local l in  fabrica.ilocales.listarLocales()) {

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
            var espsLocal =  fabrica.iespecialidades.listarEspecialidadesLocal(c.LocalID);
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
            var medsespsLocal =  fabrica.imedicos.listarMedicosEspecialidadLocal(c.LocalID, c.EspecialidadID);

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
                    //c.local =  fabrica.ilocales.obtenerLocal(c.LocalID);
                    c.ConsultaID = Convert.ToInt64(consulta.consultaID);
                    c.EspecialidadID = Convert.ToInt64(consulta.especialidadID);
                    //c.especialidad =  fabrica.iagenda.obtenerEspecialidad(c.EspecialidadID);
                    c.FuncionarioID = consulta.medID;
                    //c.medico =  fabrica.iagenda.obtenerMedico(c.FuncionarioID);


                    //c.fecha_fin = ParseDate(consulta.fecha_fin).ToUniversalTime();
                    //c.fecha_inicio = ParseDate(consulta.fecha_inicio).ToUniversalTime();



                    fabrica.iagenda.modificarConsulta(c);

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
                 fabrica.iagenda.eliminarConsulta(idCC);
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
            try
            {

           
            long idL = Convert.ToInt64(idC);
            SAREM.Shared.Entities.Consulta c =  fabrica.iagenda.obtenerConsulta(idL);
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
                medDesc = c.medico.nombre,
                cantPacientes = c.numpacientes.ToString(),
                cantPacientesEspera = c.maxpacientesespera.ToString()
            };

            return View("VerConsultaPaciente", model);
            
            }
            catch
            {
                return View("VerConsultasNew");
            }
           
        }

        [HttpGet]
        public JsonResult GetPacientes(string idC)
        {
            long idL = Convert.ToInt64(idC);
            Pacientes obj = new Pacientes();
            List<PacienteJson> pacientes = new List<PacienteJson>();

            var pacientesConsulta =  fabrica.iagenda.obtenerConsulta(idL).pacientes;
            var pacientesOrdered =  fabrica.iagenda.obtenerPacientesConsulta(idL);
            int nro = 1;
            foreach (Paciente p in pacientesOrdered)
            {

                var pC = pacientesConsulta.First(x =>( x.PacienteID == p.PacienteID &&  x.ConsultaID == idL));

                PacienteJson pj = new PacienteJson();
                Paciente pente = fabrica.ipacientes.obtenerPaciente(p.PacienteID);
                pj.PacienteID = p.PacienteID;
                pj.nombre = pente.nombre;
                pj.celular = pente.celular;
                pj.telefono = pente.telefono;
                if (pC.ausencia)
                {
                    pj.ausencia = "Si";
                }
                else
                {
                    pj.ausencia = "No";
                }
                //Cambiar por valores reales luego
                //pj.celular = "098258908";
                //pj.telefono = "29014567";
                pj.sexo = pente.sexo.ToString();
                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        (DateTime)pC.fecharegistro,
                            DateTimeKind.Utc);
                DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                pj.fechaRegistro = localVersionFIni.ToString(format);

                runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                     (DateTime)pC.turno,
                         DateTimeKind.Utc);
                localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                pj.turno = localVersionFIni.ToString(format);
                pj.numero = nro.ToString();
                nro++;
                pacientes.Add(pj);
               
            }

            obj.records = pacientes;
            return Json(obj.records, JsonRequestBehavior.AllowGet);
        }

        //Obtener pacientes fuera de lista
        [HttpGet]
        public JsonResult GetPacientesFE(string idC)
        {
            long idL = Convert.ToInt64(idC);
            Pacientes obj = new Pacientes();
            List<PacienteJson> pacientes = new List<PacienteJson>();

            var pacientesConsulta = fabrica.iagenda.obtenerConsulta(idL).pacientes;
            var pacientesOrdered = fabrica.iagenda.obtenerPacientesConsultaFueraLista(idL);
            int nro = 1;
            foreach (Paciente p in pacientesOrdered)
            {

                var pC = pacientesConsulta.First(x => (x.PacienteID == p.PacienteID && x.ConsultaID == idL));

                PacienteJson pj = new PacienteJson();
                Paciente pente = fabrica.ipacientes.obtenerPaciente(p.PacienteID);
                pj.PacienteID = p.PacienteID;
                pj.nombre = pente.nombre;
                pj.celular = pente.celular;
                pj.telefono = pente.telefono;
                if (pC.fueralista)
                {
                    pj.fueradeLista = "Si";
                }
                else
                {
                    pj.fueradeLista = "No";
                }
                //Cambiar por valores reales luego
                //pj.celular = "098258908";
                //pj.telefono = "29014567";
                pj.sexo = pente.sexo.ToString();
                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        (DateTime)pC.fecharegistro,
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
            var pacientesNotInConsulta =  fabrica.iagenda.listarPacientesNotInConsulta(idL);
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
           
            var pacientesConsulta =  fabrica.iagenda.obtenerConsulta(idL).pacientesespera;
            var pacientesEspOrdered =  fabrica.iagenda.obtenerPacientesConsultaEspera(idL);
            int nro = 1;
            foreach (Paciente p in pacientesEspOrdered)
            {
                var pC = pacientesConsulta.First(x => (x.PacienteID == p.PacienteID && x.ConsultaID == idL));
                PacienteJson pj = new PacienteJson();
                Paciente pente = fabrica.ipacientes.obtenerPaciente(p.PacienteID);
                pj.PacienteID = p.PacienteID;
                pj.nombre = pente.nombre;
                pj.celular = pente.celular;
                pj.telefono = pente.telefono;
                //Cambiar por valores reales luego
                //pj.celular = "098258908";
                //pj.telefono = "29014567";
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
        public JsonResult AddPacienteConsulta(string idC, string idP, string idCT)
        {

         
            try
            {
                long idCC = Convert.ToInt64(idC);
                short idCCT = Convert.ToInt16(idCT);

                //Chequeo que Id Paciente sea valido
                var existePac = fabrica.ipacientes.checkPaciente(idP);
                //Chequeo que Id Paciente no pertenezca a la consula ni a la lista de espera
                var notExistePacConsultaListaEspera = false;
                
                if (existePac) { 
                    
                    notExistePacConsultaListaEspera = fabrica.iagenda.pacientePerteneceConsulta(idCC, idP);
                }
                else
                {
                    return Json(new { success = false, mensaje = "El Id del Paciente ingresado no es correcto." }, JsonRequestBehavior.AllowGet);
                }

                if (existePac && notExistePacConsultaListaEspera) { 
                    
                    fabrica.iagenda.agregarConsultaPaciente(idP, idCC, idCCT, false);
                }
                else
                {

                    return Json(new { success = false, mensaje = "El Paciente ya pertenece a la Consulta o a la Lista de Espera." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { success = false , errorGrave = true, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
            }

          
        }

        [HttpPost]
        public JsonResult AddPacienteConsultaEspera(string idC, string idP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                //Chequeo que Id Paciente sea valido
                var existePac = fabrica.ipacientes.checkPaciente(idP);
                //Chequeo que Id Paciente no pertenezca a la consula ni a la lista de espera
                var notExistePacConsultaListaEspera = false;

                if (existePac)
                {

                    notExistePacConsultaListaEspera = fabrica.iagenda.pacientePerteneceConsulta(idCC, idP);
                }
                else
                {
                    return Json(new { success = false, mensaje = "El Id del Paciente ingresado no es correcto." }, JsonRequestBehavior.AllowGet);
                }

                if (existePac && notExistePacConsultaListaEspera)
                {

                    fabrica.iagenda.agregarConsultaPacienteEspera(idP, idCC);
                }
                else
                {

                    return Json(new { success = false, mensaje = "El Paciente ya pertenece a la Consulta o a la Lista de Espera." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { success = false, errorGrave = true, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
            }


        }


  

        [HttpPost]
        public JsonResult CancelPacienteConsulta(string idC, string idP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                fabrica.iagenda.cancelarConsultaPaciente(idP, idCC);
                return Json(new { success = true });
            }
            catch(Exception e)
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
                 fabrica.iagenda.eliminarPacienteConsultaLE(idP, idCC);
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

                 fabrica.iagenda.moverPacientesLEConsulta(idsP, idCC);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }

        //Parte Diario
        [HttpGet]
        public ActionResult VerParteDiario()
        {
            
            return View();
        }

        // Obtener parte diario
        [HttpGet]
        public JsonResult GetParteDiario(string MedicoID, string fecha)
        {
            try
            {
                var consultas = fabrica.iagenda.obtenerParteDiario(MedicoID,ParseDate(fecha).ToUniversalTime());
                List<ConsultaJSON> consjs = new List<ConsultaJSON>();
                foreach (SAREM.Shared.Entities.Consulta c in consultas)
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

                    consjs.Add(cjson);
                
                }
                return Json(consjs, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                
                return Json(new { success = false });
            }
           
        }

        [HttpGet]
        public ActionResult VerParteDiarioPacientes(string idC)
        {
            try {
            long idL = Convert.ToInt64(idC);
            SAREM.Shared.Entities.Consulta c = fabrica.iagenda.obtenerConsulta(idL);
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
                medDesc = c.medico.nombre,
                cantPacientes = c.numpacientes.ToString(),
                cantPacientesEspera = c.maxpacientesespera.ToString()
            };

            return View("VerPacientesParteDiario", model);
            }
            catch
            {
                return View("VerParteDiario");
            }
           
        }

        [HttpGet]
        public JsonResult GetDiagnostigoAusencia(string idC, string idP)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);

                var consulta = fabrica.iagenda.obtenerPacienteConsulta(idCC, idP);
                return Json(new { diagnostico = consulta.diagnostico, ausencia = consulta.ausencia }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult ActualizarParteDiario(string idC, string idP, string diagnostico, string ausencia)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                Boolean aBool = false;

                if (ausencia == "1")
                {
                    aBool = true;
                }

                fabrica.iagenda.actualizarParteDiario(idCC, idP, diagnostico, aBool); 
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult AgregarPacienteFE(string idC, string idP, string idCT)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                short idCCT = Convert.ToInt16(idCT);

                //Chequeo que Id Paciente sea valido
                var existePac = fabrica.ipacientes.checkPaciente(idP);
                //Chequeo que Id Paciente no pertenezca a la consula ni a la lista de espera
                var notExistePacConsultaListaEspera = false;

                if (existePac)
                {

                    notExistePacConsultaListaEspera = fabrica.iagenda.pacientePerteneceConsulta(idCC, idP);
                }
                else
                {
                    return Json(new { success = false, mensaje = "El Id del Paciente ingresado no es correcto." }, JsonRequestBehavior.AllowGet);
                }

                if (existePac && notExistePacConsultaListaEspera)
                {

                    fabrica.iagenda.agregarConsultaPaciente(idP, idCC, idCCT, true);
                }
                else
                {

                    return Json(new { success = false, mensaje = "El Paciente ya pertenece a la Consulta o a la Lista de Espera." }, JsonRequestBehavior.AllowGet);
                }

             

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {

                return Json(new { success = false, errorGrave = true, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
            }


        }

        //REVISAR.
        [HttpGet]
        public JsonResult GetParametrosConsulta()
        {
            try
            {
                //var dtp = fabrica.iagenda.obtenerParametrosConsulta();
                //return Json(new { max_pacientes = dtp.maxPacientesConsulta, max_pacientes_esp = dtp.maxPacientesListaEspera }, JsonRequestBehavior.AllowGet);
                return Json(new { max_pacientes = 10, max_pacientes_esp = 10 }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion

        #region Paciente

        //Agendar Consulta Paciente
        [HttpGet]
        public ActionResult AgendarConsultaPaciente()
        {
            var model = new SAREM.Web.Models.Consulta
            {
                local = fabrica.ilocales.listarLocales(),

            };


            return View(model);
        }



        //Ver Consultas agendadas paciente
        
        public ActionResult VerConsultasAgendadasPaciente()
        {
           return View();
        }

        //Ver Consultas canceladas paciente
        public ActionResult VerConsultasCanceladasPaciente()
        {
            return View();
        }

        //Ver Consultas para agendar
        public JsonResult GetConsultasParaAgenda(string idOrigen, string idEspecialidad, string idMedico, string fechaDesde, string fechaHasta)
        {

            var consultas = fabrica.iagenda.listarConsultasMedicoLocalEspecialidad(Convert.ToInt64(idEspecialidad), Convert.ToInt64(idOrigen), idMedico, ParseDate(fechaDesde).ToUniversalTime(),ParseDate(fechaHasta).ToUniversalTime(),paciente);
            List<ConsultaJSON> lista = new List<ConsultaJSON>();

            foreach (SAREM.Shared.Entities.Consulta c in consultas)
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

        //Ver consultas para agenda, con filtros, origen y medico
        public JsonResult GetConsultasParaAgendaOrigenMedico(string idOrigen, string idMedico, string fechaDesde, string fechaHasta)
        {

            var consultas = fabrica.iagenda.listarConsultasMedicoLocal(Convert.ToInt64(idOrigen), idMedico, ParseDate(fechaDesde).ToUniversalTime(), ParseDate(fechaHasta).ToUniversalTime(), paciente);
            List<ConsultaJSON> lista = new List<ConsultaJSON>();

            foreach (SAREM.Shared.Entities.Consulta c in consultas)
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

        //Ver consultas para agenda, con filtros, origen y especialidad
        public JsonResult GetConsultasParaAgendaOrigenEsp(string idOrigen, string idEspecialidad, string fechaDesde, string fechaHasta)
        {

            var consultas = fabrica.iagenda.listarConsultasLocalEspecialidad(Convert.ToInt64(idOrigen), Convert.ToInt64(idEspecialidad), ParseDate(fechaDesde).ToUniversalTime(), ParseDate(fechaHasta).ToUniversalTime(), paciente);
            List<ConsultaJSON> lista = new List<ConsultaJSON>();

            foreach (SAREM.Shared.Entities.Consulta c in consultas)
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


        //Ver Consultas agendadas
        public JsonResult GetConsultasPaciente() {
            Debug.WriteLine("PACIENTE SESION");
            Debug.WriteLine(paciente);
            Debug.WriteLine(tenant);

            var consultas = fabrica.iagenda.listarConsultasPaciente(paciente);
            List<ConsultaJSON> lista = new List<ConsultaJSON>();

            Debug.WriteLine("CONSULTAS JSON");
            foreach (DataConsultaPaciente c in consultas)
            {
                Debug.WriteLine("JSONCONSULTAS::"+ c.consulta.ConsultaID);
                ConsultaJSON cjson = new ConsultaJSON();

                cjson.idC = c.consulta.ConsultaID.ToString();
                cjson.origen = c.consulta.local.nombre;
                cjson.especialidad = c.consulta.especialidad.descripcion;
                cjson.medico = c.consulta.medico.nombre;

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

                //var consulta = fabrica.iagenda.obtenerTurno(c.consulta.ConsultaID, paciente);

             
                //if (consulta != null)
               // {
                    //DateTime turno = consulta.turno ?? DateTime.UtcNow;

                if (c.turno != null)
                {
                    DateTime turno = c.turno ?? DateTime.UtcNow;
                    runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                    turno,
                    DateTimeKind.Utc);
                    localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();

                    cjson.turno = localVersionFIni.ToString(format);


                }
                   
                //}
            
                lista.Add(cjson);
            }

            var aux = new GetConsultasJSON
            {

                records = lista
            };
          

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        //Ver Consultas Canceladas paciente
        public JsonResult GetConsultasPacienteCancel()
        {

            var consultas = fabrica.iagenda.listarConsultasCanceladasPaciente(paciente);
            List<ConsultaJSON> lista = new List<ConsultaJSON>();

            foreach (SAREM.Shared.Entities.Consulta c in consultas)
            {
                ConsultaJSON cjson = new ConsultaJSON();

                cjson.idC = c.ConsultaID.ToString();
                cjson.origen = fabrica.ilocales.obtenerLocal(c.LocalID).nombre;
                cjson.especialidad = fabrica.iespecialidades.obtenerEspecialidad(c.EspecialidadID).descripcion;
                cjson.medico = fabrica.imedicos.obtenerMedico(c.FuncionarioID).nombre;

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

        [HttpPost]
        public JsonResult CancelarConsulta(string idC)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                //Cambiar luego id paciente
                var pertenece = fabrica.iagenda.perteneceConsulta(paciente, idCC);
                if (pertenece) { 
                    
                    fabrica.iagenda.cancelarConsultaPaciente(paciente, idCC);
                }
                else
                {
                    fabrica.iagenda.eliminarPacienteConsultaLE(paciente, idCC);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }

        [HttpPost]
        public JsonResult AgregarPacienteConsulta(string idC, string idCT)
        {


            try
            {
                long idCC = Convert.ToInt64(idC);
                short idCCT = Convert.ToInt16(idCT);
                //DateTime? turnoAux = fabrica.iagenda.agregarConsultaPaciente("14", idCC, idCCT);

                fabrica.iagenda.agregarConsultaPaciente(paciente, idCC, idCCT, false);
                //if (turnoAux != null) {
                    
                //    DateTime turno = turnoAux ?? DateTime.UtcNow;
                //    String format = "dd/MM/yyyy HH:mm";
                //    DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                //            turno,
                //            DateTimeKind.Utc);
                //    DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                //    var fecha = localVersionFIni.ToString(format);

                //    return Json(new { success = true , turno = fecha});
                
                //} else {

                //    return Json(new { success = true , turno = "LE" });
                //}
                return Json(new { success = true });
            }
            catch (ExcepcionMaxPacientesConsulta)
            {
                return Json(new { errorCuposTomados = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }


        }

        public JsonResult ConsultarDisponibilidadConsulta(string idC)
        {
            try
            {
                DataConsulta dc = fabrica.iagenda.consultarDisponibilidadConsulta(Convert.ToInt64(idC));

                return Json(new { disponiblidadConsulta = dc.lugaresLibreConsulta, disponibilidadLE = dc.lugaresLibresListaEspera }, JsonRequestBehavior.AllowGet);
            }
            catch
            {

                return Json(new { falla = true });
            }
        }

        [HttpPost]
        public ActionResult AddPacienteConsultaEsperaPaciente(string idC)
        {

             //return RedirectToAction("AddPacienteConsultaEspera", "Consulta", new {idC  = idC, idP = "14" });
            try
            {
                long idCC = Convert.ToInt64(idC);
                fabrica.iagenda.agregarConsultaPacienteEspera(paciente, idCC);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }


        }



        public JsonResult GetMedicosOrigen(string idLocalidad)
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Medico m in fabrica.imedicos.listarMedicosLocal(Convert.ToInt64(idLocalidad)))
            {

                medicos.Add(new SelectListItem { Text = m.nombre, Value = m.FuncionarioID });
            }
            return Json(new SelectList(medicos, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }

       
        #endregion
       

    }
}
