
namespace SocTube.Core.Models.Domains
{
    public class Settings
    {
        public int Id { get; set; }
        public bool LinkVisible { get; set; }
        public string LinksTheme { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
} 