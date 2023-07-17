using System.ComponentModel.DataAnnotations;

namespace SocTube.Core.Models.Domains
{
    public class Link
    {
        public int Id { get; set; }
        [Display(Name ="Nazwa")]
        public string Name { get; set; }
        [Display(Name = "Adres Url")]
        public string Url { get; set; }
        public string UserId { get; set; }
        public Profile Profile { get; set; }
    }
}
