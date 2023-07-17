using Microsoft.AspNetCore.Http;
using SocTube.Core.Models.Domains;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocTube.Core.ViewModels
{
    public class UserInputViewModel
    {
        public Profile UserProfile { get; set; }
        public Link UserLinks { get; set; }
        public IEnumerable<Settings> UserSettings { get; set; }
        public SocialMedia UserSocialMedia { get; set; }
    }
}
