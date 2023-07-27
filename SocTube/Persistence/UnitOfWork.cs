using SocTube.Core.Models.Domains;
using SocTube.Persistence.Repositories;

namespace SocTube.Persistence
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Profile _profile;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Profile = new ProfilesRepository(context);
        }

        public ProfilesRepository Profile { get; set; }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}
