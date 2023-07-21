using System.ComponentModel.DataAnnotations;

namespace SocTube.Core.Models.Domains
{
    public class Link
    {
        public int Id { get; set; }
        [Display(Name ="Nazwa")]
        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Name { get; set; }
        [Display(Name = "Adres Url")]
        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Url { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Widoczność na stronie")]
        public string ButtonStyle { get; set; }

        public bool IsVisible { get; set; }
        public Profile Profile { get; set; }
    }
}
