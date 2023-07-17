using SocTube.Core.Models.Domains;
using System.Collections.Generic;

namespace SocTube.Core.ViewModels
{
    public class UserViewModel
    {
        public Profile UserProfile { get; set; }
        public IEnumerable<Link> UserLinks { get; set; }
        public Settings UserSettings { get; set; }
        public SocialMedia UserSocialMedia { get; set; }
    }
}
