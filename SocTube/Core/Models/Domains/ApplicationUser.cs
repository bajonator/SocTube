using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SocTube.Core.Models.Domains
{
    public class ApplicationUser : IdentityUser
    {
        public Profile Profile { get; set; }
    }
}
