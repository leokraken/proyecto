using DHTMLX.Scheduler;
using DHTMLX.Scheduler.Data;
using SAREM.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAREM.Web.Controllers
{
    //Este controlador conviene renombrarlo, la idea es ver como funciona.
    public class CalendarController : Controller
    {
        //se crea en el contexto test, pero es configurable, fase de integracion.
        public static string tenant = "test";
        IDALAgenda idal = new DALAgenda(tenant);


        public ActionResult Index()
        {
            var scheduler = new DHXScheduler(this);
            scheduler.Skin = DHXScheduler.Skins.Flat;
            //ajustar limites, consultar a estudiantes de RM
            scheduler.Config.first_hour = 0;
            scheduler.Config.last_hour = 22;
            scheduler.EnableDynamicLoading(SchedulerDataLoader.DynamicalLoadingMode.Month);
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;

            return View(scheduler);
        }

        //se cargan los datos con la dal.
        public ContentResult Data(DateTime from, DateTime to)
        {
            var apps = idal.listarConsultas().ToList();
            return new SchedulerAjaxData(apps);
        }
        //faltan operaciones save, etc.
    }
}