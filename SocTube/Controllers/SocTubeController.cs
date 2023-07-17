using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocTube.Core.Models.Domains;
using SocTube.Core.ViewModels;
using SocTube.Persistence.Extensions;
using SocTube.Persistence;
using SocTube.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using SocTube.Data.Migrations;
using Microsoft.CodeAnalysis.Differencing;
using SocTube.Core;
using System.Collections.Generic;
using System;

namespace SocTube.Controllers
{
    [Authorize]
    public class SocTubeController : Controller
    {
        private ProfilesRepository _profilesRepository;

        public SocTubeController(ApplicationDbContext context)
        {
            _profilesRepository = new ProfilesRepository(context);

        }
        public IActionResult Users()
        {
            var viewModel = new UsersViewModel
            {
                UserProfiles = _profilesRepository.Get(),
                UserSocialMedia = _profilesRepository.GetMedias()
            };

            return View("_UserList", viewModel);
        }
        public IActionResult Links()
        {
            var userId = User.GetUserId();
            var media = _profilesRepository.GetUser(userId);
            var viewModel = new UsersViewModel
            {
                UserProfiles = _profilesRepository.Get(),
                UserLinks = _profilesRepository.GetLinks(userId),
                UserSettings = _profilesRepository.GetSettings(),
            };
            if (media != null)
                viewModel.UserSocialMedia = _profilesRepository.GetMedias();


            return View("_UserLinks", viewModel);
        }
        public IActionResult UserInput()
        {
            var userId = User.GetUserId();

            var userInput = new UserViewModel
            {
                UserProfile = _profilesRepository.GetUser(userId),
                UserLinks = _profilesRepository.GetLinks(userId),
                UserSocialMedia = _profilesRepository.GetSocialMedia(userId),
                //UserSettings = _profilesRepository.GetSettings(),
            };
            userInput.UserProfile.SocialMedia = _profilesRepository.GetListMedia(userId);
            return View(userInput);
        }
        public IActionResult UserLinks()
        {
            var userId = User.GetUserId();
            var media = _profilesRepository.GetUser(userId);

            var userInput = new UserViewModel
            {
                UserProfile = media
            };

            //if (media != null)
                //userInput.UserSocialMedia = _profilesRepository.GetSocialMedia(userId);
            //else userInput.UserSocialMedia = new SocialMedia();

            return View(userInput);
        }
        [HttpGet]
        [Route("SocTube/UserPage/{UserId}")]
        public IActionResult UserPage(string userId, string userName, SocialMedia userViewModel, Link userLink)
        {
            var userInput = new UserViewModel
            {
                UserProfile = _profilesRepository.GetUserProfiles(userId),
                UserLinks = _profilesRepository.GetLinks(userId),
                UserSocialMedia = _profilesRepository.GetSocialMedia(userId)
            };
            userInput.UserProfile.SocialMedia = _profilesRepository.GetListMedia(userId);

            return View(userInput);
        }
        [HttpPost]
        public IActionResult SaveUser(Profile model, SocialMedia socialMedia, IFormFile profilePicture)
        {
            var userId = User.GetUserId();

            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                SavePicture(model, profilePicture, userId);

                var userProfile = new Profile
                {
                    Id = model.Id,
                    UserId = userId,
                    UserName = model.UserName,
                    ContactEmail = model.ContactEmail,
                    Description = model.Description,
                    ProfileImage = model.ProfileImage,
                };
                var userSocialMedia = new SocialMedia
                {
                    Id = socialMedia.Id,
                    UserId = userId,
                    YouTube = socialMedia.YouTube,
                    Instagram = socialMedia.Instagram,
                    Facebook = socialMedia.Facebook,
                    Tiktok = socialMedia.Tiktok,
                    Twitch = socialMedia.Twitter,
                    Twitter = socialMedia.Twitter,
                    Github = socialMedia.Github,
                };        

                if (model.Id > 0)
                    _profilesRepository.Update(userProfile);
                else
                    _profilesRepository.Add(userProfile);

                if (socialMedia.Id > 0)
                    _profilesRepository.UpdateSocial(userSocialMedia, userProfile.Id);
                else
                    _profilesRepository.AddSocial(userSocialMedia);
                

                return RedirectToAction("Users");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveLink(UserInputViewModel model)
        {
            var userId = User.GetUserId();

            ModelState.Remove("UserLinks.Id");

            if (!ModelState.IsValid)
            {
                var vm = new UserInputViewModel
                {
                    UserProfile = model.UserProfile,
                    UserSocialMedia = model.UserSocialMedia,
                    UserLinks = model.UserLinks,
                };
                return View("_UserLinks", vm);
            }
            if (model.UserLinks.Id == 0)
            {
                model.UserLinks.UserId = userId;
                _profilesRepository.AddLink(model.UserLinks);
            }
            else
            {
                model.UserLinks.UserId = userId;
                _profilesRepository.UpdateLink(model.UserLinks);
            }
            return RedirectToAction("Users");
        }

        private void SavePicture(Profile model, IFormFile profilePicture, string userId)
        {
            if (model.ProfileImage == null && profilePicture != null)
                _profilesRepository.AddPhoto(model, profilePicture);
            else
            {
                var existingProfile = _profilesRepository.GetUser(userId);
                if (existingProfile != null)
                    model.ProfileImage = existingProfile.ProfileImage;
            }
        }
    }
}
