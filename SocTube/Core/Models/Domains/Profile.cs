using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocTube.Core.Models.Domains
{
    public class Profile
    {
        public Profile()
        {
            Links = new Collection<Link>();
            SocialMedia = new Collection<SocialMedia>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Twoja Nazwa")]
        public string UserName { get; set; }
        [Display(Name = "Twój Opis")]
        public string Description { get; set; }
        [Display(Name = "Email Kontaktowy")]
        public string ContactEmail { get; set; }
        public ICollection<Link> Links { get; set; }
        public ICollection<SocialMedia> SocialMedia { get; set; }
        public byte[] ProfileImage { get; set; }
        public ApplicationUser User { get; set; }
    }
}