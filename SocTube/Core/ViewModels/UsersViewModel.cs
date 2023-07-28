using SocTube.Core.Models.Domains;
using System.Collections.Generic;

namespace SocTube.Core.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<Profile> UserProfiles { get; set; }
        public IEnumerable<Link> UserLinks { get; set; }
        public IEnumerable<SocialMedia> UserSocialMedia { get; set; }
    }
}
