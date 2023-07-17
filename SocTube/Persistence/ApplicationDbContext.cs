using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocTube.Core.Models.Domains;
using System.Reflection.Emit;

namespace SocTube.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Link> Links { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SocialMedia> SocialMedia { get; set; }
    }
}
