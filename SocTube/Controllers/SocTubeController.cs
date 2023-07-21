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
                UserSocialMedia = _profilesRepository.GetSocialMedia(userId),
                UserLinks = new Link()
            };
            if (userInput.UserProfile != null)
                userInput.UserProfile.SocialMedia = _profilesRepository.GetListMedia(userId);
            if (userInput.UserProfile != null)
                userInput.UserProfile.Links = _profilesRepository.GetLinks(userId);

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
                UserLinks = userLink,
                UserSocialMedia = _profilesRepository.GetSocialMedia(userId)
            };
            userInput.UserProfile.SocialMedia = _profilesRepository.GetListMedia(userId);
            userInput.UserProfile.Links = _profilesRepository.GetLinks(userId);

            return View(userInput);
        }
        [HttpPost]
        public IActionResult SaveUser(Profile model, IFormFile profilePicture)
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

                if (model.Id > 0)
                    _profilesRepository.Update(userProfile);
                else
                    _profilesRepository.Add(userProfile);

                return RedirectToAction("UserInput");
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult SaveMedia(SocialMedia socialMedia)
        {
            var userId = User.GetUserId();
            var profile = _profilesRepository.GetUser(userId);

            ModelState.Remove("Profile.Id");
            if (ModelState.IsValid)
            {

                var userSocialMedia = new SocialMedia
                {
                    UserId = userId,
                    YouTube = socialMedia.YouTube,
                    Instagram = socialMedia.Instagram,
                    Facebook = socialMedia.Facebook,
                    Tiktok = socialMedia.Tiktok,
                    Twitch = socialMedia.Twitter,
                    Twitter = socialMedia.Twitter,
                    Github = socialMedia.Github,
                };

                if (profile.Id > 0)
                    _profilesRepository.UpdateSocial(userSocialMedia, profile.Id);
                else
                    _profilesRepository.AddSocial(userSocialMedia);


                return RedirectToAction("UserInput");
            }

            return View(socialMedia);
        }
        [HttpPost]
        public IActionResult SaveLink(Link model)
        {
            model.UserId = User.GetUserId();

            ModelState.Remove("Id");

            if (!ModelState.IsValid)
            {
                var vm = new Link
                {
                    UserId = User.GetUserId(),
                    Name = model.Name,
                    Url = model.Url,
                    IsVisible = model.IsVisible,
                    ButtonStyle = model.ButtonStyle,
                };
                return View("_UserLinks", vm);
            }

            if (model.Id == 0)
                _profilesRepository.AddLink(model);
            else
                _profilesRepository.UpdateLink(model);

            return RedirectToAction("UserInput");
        }
        [HttpPost]
        public IActionResult SaveLinkStyle(Link model)
        {
            var userId = User.GetUserId();
            var userLinks = _profilesRepository.GetLinks(userId);
            try
            {
                foreach (var link in userLinks)
                {
                    link.ButtonStyle = model.ButtonStyle;
                    _profilesRepository.UpdateLink(link);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult EditLink(int linkId)
        {
            var link = _profilesRepository.FindLink(linkId);
            var linkmodel = new Link
            {
                Id = linkId,
                Name = link.Name,
                Url = link.Url,
                UserId = link.UserId,
                IsVisible = link.IsVisible,
            };

            return View("_UserLinks", linkmodel);
        }
        [HttpPost]
        public ActionResult DeleteLink(int id)
        {
            try
            {
                var userId = User.GetUserId();
                _profilesRepository.DeleteLink(id, userId);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = true });
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
