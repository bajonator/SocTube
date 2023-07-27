using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using SocTube.Core.Models.Domains;
using SocTube.Data.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SocTube.Persistence.Repositories
{
    public class ProfilesRepository
    {
        private ApplicationDbContext _context;
        public ProfilesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Profile> Get()
        {
            return _context.Profiles.Include(x => x.Links).Include(x => x.SocialMedia).ToList();
        }

        public List<Link> GetLinks(string userId)
        {
            return _context.Links.Where(x => x.UserId == userId).ToList();
        }
        public Link GetLink(string userId)
        {
            return _context.Links.FirstOrDefault(x => x.UserId == userId);
        }
        public Link GetActiveStyle(string userId)
        {
            return _context.Links.FirstOrDefault(x => x.UserId == userId && x.ButtonStyle != null);
        }
        public IEnumerable<Settings> GetSettings()
        {
            return _context.Settings.ToList();
        }
        public IEnumerable<SocialMedia> GetMedias()
        {
            return _context.SocialMedia.ToList();
        }
        public SocialMedia GetSocialMedia(string userId)
        {
            return _context.SocialMedia.FirstOrDefault(x => x.UserId == userId);
        }

        public Profile GetUser(string userId)
        {
            return _context.Profiles.Include(x => x.SocialMedia).Include(x => x.Links).SingleOrDefault(x => x.UserId == userId);
        }

        public void Add(Profile userProfile)
        {
            _context.Profiles.Add(userProfile);
        }

        public void AddPhoto(Profile userProfile, IFormFile photo)
        {
            if (photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    photo.CopyTo(memoryStream);
                    userProfile.ProfileImage = memoryStream.ToArray();
                }
            }
        }
        public void Update(Profile userProfile)
        {
            var existingProfile = _context.Profiles.Find(userProfile.Id);
            if (existingProfile != null)
            {
                existingProfile.UserName = userProfile.UserName;
                existingProfile.ContactEmail = userProfile.ContactEmail;
                existingProfile.Description = userProfile.Description;
                existingProfile.ProfileImage = userProfile.ProfileImage;
                existingProfile.SocialMedia = userProfile.SocialMedia;
            }
        }

        public void AddSocial(SocialMedia socialMedia)
        {
            _context.SocialMedia.Add(socialMedia);
        }

        public void AddLink(Link userLinks)
        { 
            _context.Links.Add(userLinks);
        }

        public void UpdateLink(Link userLinks)
        {
            var existingProfile = _context.Links.FirstOrDefault(x => x.UserId == userLinks.UserId && x.Id == userLinks.Id);
            if (existingProfile != null)
            {
                existingProfile.Url = userLinks.Url;
                existingProfile.Name = userLinks.Name;
                existingProfile.IsVisible = userLinks.IsVisible;
                existingProfile.ButtonStyle = userLinks.ButtonStyle;
            }
            else
            {
                AddLink(userLinks);
            }
        }

        public Profile GetUserProfiles(string userId)
        {
            return _context.Profiles.FirstOrDefault(x => x.UserId == userId);
        }

        public void UpdateSocial(SocialMedia socialMedia, int id)
        {
            var existingProfile = _context.SocialMedia.FirstOrDefault(x => x.UserId == socialMedia.UserId);

            if (existingProfile != null)
            {
                existingProfile.YouTube = socialMedia.YouTube;
                existingProfile.Instagram = socialMedia.Instagram;
                existingProfile.Facebook = socialMedia.Facebook;
                existingProfile.Tiktok = socialMedia.Tiktok;
                existingProfile.Twitch = socialMedia.Twitch;
                existingProfile.Twitter = socialMedia.Twitter;
                existingProfile.Github = socialMedia.Github;
            }
            else
            {
                AddSocial(socialMedia);
            }
        }

        public List<SocialMedia> GetListMedia(string userId)
        {
            return _context.SocialMedia.Where(x => x.UserId == userId).ToList();
        }

        public Link FindLink(int linkId)
        {
            return _context.Links.Find(linkId);
        }

        public void DeleteLink(int linkId, string userId)
        {
            var link = _context.Links.Find(linkId);
            _context.Links.Remove(link);
        }
    }
}
