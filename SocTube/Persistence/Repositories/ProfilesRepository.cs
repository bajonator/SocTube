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
            _context.SaveChanges();
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
            _context.SaveChanges();
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
                _context.SaveChanges();
            }
        }

        public void AddSocial(SocialMedia socialMedia)
        {
            _context.SocialMedia.Add(socialMedia);
            _context.SaveChanges();
        }

        public Link GetLink(string userId)
        {
            return _context.Links.FirstOrDefault(x => x.UserId == userId);
        }

        public void AddLink(Link userLinks)
        {
            _context.Links.Add(userLinks);
            userLinks.UserId = userLinks.UserId;
            _context.SaveChanges();
        }

        public void UpdateLink(Link userLinks)
        {
            var existingProfile = _context.Links.FirstOrDefault(x => x.UserId == userLinks.UserId);
            if (existingProfile != null)
            {
                existingProfile.Url = userLinks.Url;
                existingProfile.Name = userLinks.Name;

                _context.SaveChanges();
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
            var existingProfile = _context.SocialMedia.Find(id);

            if (existingProfile != null)
            {
                existingProfile.YouTube = socialMedia.YouTube;
                existingProfile.Instagram = socialMedia.Instagram;
                existingProfile.Facebook = socialMedia.Facebook;
                existingProfile.Tiktok = socialMedia.Tiktok;
                existingProfile.Twitch = socialMedia.Twitch;
                existingProfile.Twitter = socialMedia.Twitter;
                existingProfile.Github = socialMedia.Github;
                _context.SaveChanges();
            }
        }

        public List<SocialMedia> GetListMedia(string userId)
        {
            return _context.SocialMedia.Where(x => x.UserId == userId).ToList();
        }
    }
}
