using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SAREM.Backoffice.Models
{
    // Para agregar datos de perfil al usuario, agregue más propiedades a la clase ApplicationUser. Para obtener más información, visite http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string Hometown { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar notificaciones de usuario personalizadas aquí
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<SAREM.Shared.Entities.Paciente> Pacientes { get; set; }

        public System.Data.Entity.DbSet<SAREM.Shared.Entities.Medico> Medicos { get; set; }

        public System.Data.Entity.DbSet<SAREM.Shared.Entities.Pais> Pais { get; set; }

        public System.Data.Entity.DbSet<identifierDomain> identifierDomains { get; set; }
    }
}