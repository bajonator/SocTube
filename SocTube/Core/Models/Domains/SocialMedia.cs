using System;
using System.Diagnostics.CodeAnalysis;

namespace SocTube.Core.Models.Domains
{
    public class SocialMedia
    {
        public int Id { get; set; }
        public string YouTube { get; set; }
        public string Twitch { get; set; }
        public string Instagram { get; set; }
        public string Tiktok { get; set; }
        public string Twitter { get; set; }
        public string Github { get; set; }
        public string Facebook { get; set; }
        public string UserId { get; set; }
        public Profile Profile { get; set; }
        //public ApplicationUser User { get; set; }
    }
}