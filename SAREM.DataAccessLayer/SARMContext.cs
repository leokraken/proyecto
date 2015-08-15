﻿using SAREM.Shared.Entities;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SAREM.DataAccessLayer
{
    public class SARMContext : DbContext
    { 
        public string tenant;
        static string con = ConfigurationManager.ConnectionStrings["sarem"].ConnectionString;
        static DbConnection connection = new SqlConnection(con);

        public DbSet<AgendaEvento> agendaeventos { get; set; }
        public DbSet<Comunicacion> comunicaciones { get; set; }
        //normal especial
        public DbSet<Consulta> consultas { get; set; }
        public DbSet<Especialidad> especialidades { get; set; }
        public DbSet<Evento> eventos { get; set; }
        public DbSet<Local> locales { get; set; }
        //medicos
        public DbSet<Funcionario> funcionarios { get; set; }

        public DbSet<Paciente> pacientes { get; set; }
        public DbSet<PacienteConsultaAgenda> consultasagendadas { get; set; }
        public DbSet<PacienteConsultaCancelar> consultascanceladas { get; set; }
        public DbSet<PacienteEvento> eventospacientes { get; set; }
        public DbSet<Parte> partes { get; set; }
        public DbSet<Rango> rangos { get; set; }
        public DbSet<Referencia> referencias { get; set; }
        public DbSet<Pais> paises { get; set; }
        public DbSet<EventoPacienteComunicacion> eventopacientecomunicacion { get; set; }
        public DbSet<MedicoLocal> medicolocal { get; set; }
        public DbSet<PacienteConsultaEspera> pacienteespera { get; set; }
        public DbSet<PacienteConsultaAusencia> pacienteausentes { get; set; }


        private SARMContext(DbCompiledModel model, string name): base("Name=sarem", model)
        {
            tenant = name;
            Database.SetInitializer<SARMContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
        }

        public static ConcurrentDictionary<string, DbCompiledModel> modelCache = new ConcurrentDictionary<string, DbCompiledModel>();

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


        public static SARMContext getTenant(string schemaName)
        {
            var builder = new DbModelBuilder();
            builder.Entity<AgendaEvento>().ToTable("NotificacionesConsultas", schemaName)
                .HasKey(k => k.ID);
            builder.Entity<Comunicacion>().ToTable("Comunicaciones", schemaName);

            builder.Entity<Consulta>()
                .ToTable("Consultas", schemaName);
            
            builder.Entity<Pais>().ToTable("Paises", schemaName);

            builder.Entity<Especialidad>().ToTable("Especialidades", schemaName);
            //estatico notificacion
            builder.Entity<Evento>()
                .Map<EventoEstatico>(ee => ee.Requires("TipoEvento").HasValue(0))
                .Map<EventoNotificacion>(en => en.Requires("TipoEvento").HasValue(1))
                .ToTable("Eventos", schemaName);
            //medicos, etc
            builder.Entity<Funcionario>().ToTable("Funcionarios", schemaName);
            builder.Entity<Paciente>().ToTable("Pacientes", schemaName);
            builder.Entity<EventoPacienteComunicacion>().ToTable("EventoPacienteComunicacion", schemaName);


            builder.Entity<PacienteConsultaCancelar>().ToTable("ConsultasCanceladas", schemaName);
            builder.Entity<Paciente>()
                .HasMany<PacienteConsultaCancelar>(p => p.canceladas)
                .WithRequired(pc => pc.paciente);

            
            builder.Entity<PacienteEvento>().ToTable("EventosPaciente", schemaName)
                .HasKey(k => new { k.PacienteID, k.EventoID, k.ComunicacionID });
            builder.Entity<Parte>().ToTable("Partes", schemaName);
            builder.Entity<Rango>().ToTable("Rangos", schemaName);
            builder.Entity<Referencia>().ToTable("Referencias", schemaName)
                .HasKey(k => new { k.PacienteID, k.FuncionarioID});


            builder.Entity<PacienteConsultaAgenda>().ToTable("PacienteConsulta", schemaName)
                .HasRequired(x => x.consulta).WithMany(x => x.pacientes);

            builder.Entity<Medico>()
                .HasMany<Especialidad>(m => m.especialidades)
                .WithMany(e => e.medicos)
                .Map(me => {
                    me.MapLeftKey("MedicoID");
                    me.MapRightKey("EspecialidadID");
                    me.ToTable("MedicosEspecialidades", schemaName);
                });

            builder.Entity<Local>().ToTable("Locales", schemaName);

            builder.Entity<Local>()
                .HasMany<Especialidad>(l => l.especialidades)
                .WithMany(e => e.locales)
                .Map(me =>{
                    me.MapLeftKey("LocalID");
                    me.MapRightKey("EspecialidadID");
                    me.ToTable("LocalesEspecialidades", schemaName);
                });
            builder.Entity<EventoEstatico>()
                .HasMany<Rango>(e => e.rangos)
                .WithMany(r => r.eventos)
                .Map(me =>
                {
                    me.MapLeftKey("EventoID");
                    me.MapRightKey("RangoID");
                    me.ToTable("EventoRangos", schemaName);
                });

            builder.Entity<Medico>()
                .HasMany<Local>(e => e.locales)
                .WithMany(r => r.medicos)
                .Map(me =>
                {
                    me.MapLeftKey("MedicoID");
                    me.MapRightKey("LocalID");
                    me.ToTable("MedicoLocal", schemaName);
                });

            /*
            builder.Entity<Paciente>()
                .HasMany<Consulta>(p => p.consultasausentes)
                .WithMany(c => c.pacientesausencias)
                .Map(me =>
                {
                    me.MapLeftKey("ConsultaID");
                    me.MapRightKey("PacienteID");
                    me.ToTable("AusenciasPacientes", schemaName);
                });*/
            

            builder.Entity<Evento>()
                .HasMany<EventoPacienteComunicacion>(e=> e.pacientes);
            builder.Entity<Paciente>()
                .HasMany<EventoPacienteComunicacion>(e => e.eventos);

            builder.Entity<PacienteConsultaEspera>().ToTable("PacientesEspera", schemaName);
            builder.Entity<PacienteConsultaAusencia>().ToTable("PacientesAusentes", schemaName);

            
            var model = builder.Build(new SqlConnection(con));
            DbCompiledModel compModel = model.Compile();
            var compiledModel = modelCache.GetOrAdd(schemaName, compModel);
            SARMContext ret = new SARMContext(compiledModel, schemaName);
            return ret;
        }


        public static void createTenant(string tenantSchema)
        {
            try
            {
                using (var ctx = getTenant(tenantSchema))
                {
                    if (!ctx.Database.Exists())
                    {
                        ctx.Database.Create();
                    }
                    else
                    {
                        try
                        {
                            var createScript = ((IObjectContextAdapter)ctx).ObjectContext.CreateDatabaseScript();
                            ctx.Database.ExecuteSqlCommand(createScript);
                        }
                        catch (Exception E)
                        {
                            throw new Exception("El esquema ya existe");
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public SARMContext():base(con)
        //{
        //    Database.SetInitializer<SARMContext>(null);
        //}

    }

    public class SAREMAdminContext : DbContext
    {
        static string con = ConfigurationManager.ConnectionStrings["sarem"].ConnectionString;

        public SAREMAdminContext():base(con)
        {
            Database.SetInitializer<SAREMAdminContext>(null);
        }
        public void dropSchema(string schema)
        {
            string drop = "drop table [{0}].[{1}]";
            string tables = @"SELECT TABLE_NAME FROM information_schema.tables where TABLE_SCHEMA=@schema";
            string alters = @"select concat('alter table [', @schema, '].[', table_name, '] drop constraint ', constraint_name, ';') from information_schema.table_constraints where table_schema=@schema and constraint_type='FOREIGN KEY'";
            var schemaslist = this.Database.SqlQuery<string>(alters, new SqlParameter("@schema", schema)).ToListAsync(); //.ToList();

            foreach (var s in schemaslist.Result)
            {
                Database.ExecuteSqlCommand(s);
                Console.WriteLine(s.ToString());
            }

            var tablelist = this.Database.SqlQuery<string>(tables, new SqlParameter("@schema", schema)).ToListAsync();
            foreach (var t in tablelist.Result)
            {

                Console.WriteLine(String.Format(drop, schema, t));
                Database.ExecuteSqlCommand(String.Format(drop, schema, t));
            }
        }
    }

}
