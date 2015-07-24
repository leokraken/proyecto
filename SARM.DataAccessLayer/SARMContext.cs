using SAREM.Shared;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SARM.DataAccessLayer
{
    public class SARMContext : DbContext
    {
        public string tenant;
        static string con = @"Data Source=SLAVE-PC\SQLEXPRESS;Initial Catalog=sarem;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
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


        private SARMContext(DbCompiledModel model, string name): base(con, model)
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
            builder.Entity<AgendaEvento>().ToTable("notificaciones_consultas", schemaName)
                .HasKey(k => k.ID);
            builder.Entity<Comunicacion>().ToTable("Comunicaciones", schemaName);

            builder.Entity<Consulta>()
                .ToTable("Consultas", schemaName);

            builder.Entity<Consulta>()
                .HasOptional(c => c.paciente)
                .WithRequired(cp => cp.consulta);


            builder.Entity<Especialidad>().ToTable("Especialidades", schemaName);
            //estatico notificacion
            builder.Entity<Evento>()
                .Map<EventoEstatico>(ee => ee.Requires("TipoEvento").HasValue(0))
                .Map<EventoNotificacion>(en => en.Requires("TipoEvento").HasValue(1))
                .ToTable("Eventos", schemaName);
            //medicos, etc
            builder.Entity<Funcionario>().ToTable("Funcionarios", schemaName);
            builder.Entity<Paciente>().ToTable("Pacientes", schemaName);
            builder.Entity<Paciente>()
                .HasMany<PacienteConsultaAgenda>(p =>p.agendadas)
                .WithRequired(pc => pc.paciente);
            builder.Entity<Paciente>()
                .HasMany<PacienteConsultaCancelar>(p => p.canceladas)
                .WithRequired(pc => pc.paciente);


            builder.Entity<PacienteConsultaAgenda>().ToTable("consultas_agendadas", schemaName)
                .HasKey(k => new { k.PacienteID, k.ConsultaID });

            builder.Entity<PacienteConsultaCancelar>().ToTable("consultas_canceladas", schemaName)
                .HasKey(k => new { k.PacienteID, k.ConsultaID });
            builder.Entity<PacienteEvento>().ToTable("eventos_paciente", schemaName)
                .HasKey(k => new { k.PacienteID, k.EventoID, k.ComunicacionID });
            builder.Entity<Parte>().ToTable("Partes", schemaName);
            builder.Entity<Rango>().ToTable("Rangos", schemaName);
            builder.Entity<Referencia>().ToTable("Referencias", schemaName)
                .HasKey(k => new { k.PacienteID, k.FuncionarioID});


            var model = builder.Build(connection);
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
                        var createScript = ((IObjectContextAdapter)ctx).ObjectContext.CreateDatabaseScript();
                        ctx.Database.ExecuteSqlCommand(createScript);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
