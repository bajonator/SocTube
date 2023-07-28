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
        private UnitOfWork _unitOfWork;

        public SocTubeController(ApplicationDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }
        public IActionResult Users()
        {
            var viewModel = new UsersViewModel
            {
                UserProfiles = _unitOfWork.Profile.Get(),
                UserSocialMedia = _unitOfWork.Profile.GetMedias()
            };

            return View("_UserList", viewModel);
        }
        public IActionResult Links()
        {
            var userId = User.GetUserId();
            var media = _unitOfWork.Profile.GetUser(userId);
            var viewModel = new UsersViewModel
            {
                UserProfiles = _unitOfWork.Profile.Get(),
                UserLinks = _unitOfWork.Profile.GetLinks(userId),
            };
            if (media != null)
                viewModel.UserSocialMedia = _unitOfWork.Profile.GetMedias();


            return View("_UserLinks", viewModel);
        }
        public IActionResult UserInput()
        {
            var userId = User.GetUserId();

            var userInput = new UserViewModel
            {
                UserProfile = _unitOfWork.Profile.GetUser(userId),
                UserSocialMedia = _unitOfWork.Profile.GetSocialMedia(userId),
                UserLinks = new Link()
            };
            if (userInput.UserProfile != null)
                userInput.UserProfile.SocialMedia = _unitOfWork.Profile.GetListMedia(userId);
            if (userInput.UserProfile != null)
                userInput.UserProfile.Links = _unitOfWork.Profile.GetLinks(userId);

            return View(userInput);
        }
        public IActionResult UserLinks()
        {
            var userId = User.GetUserId();
            var media = _unitOfWork.Profile.GetUser(userId);

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
        public IActionResult UserPage(string userId, Link userLink)
        {
            var userInput = new UserViewModel
            {
                UserProfile = _unitOfWork.Profile.GetUserProfiles(userId),
                UserLinks = userLink,
                UserSocialMedia = _unitOfWork.Profile.GetSocialMedia(userId)
            };
            userInput.UserProfile.SocialMedia = _unitOfWork.Profile.GetListMedia(userId);
            userInput.UserProfile.Links = _unitOfWork.Profile.GetLinks(userId);

            return View(userInput);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    _unitOfWork.Profile.Update(userProfile);
                else
                    _unitOfWork.Profile.Add(userProfile);
                _unitOfWork.Complete();

                return RedirectToAction("UserInput");
            }

            return View("_UserData");
        }
        [HttpPost]
        public IActionResult SaveMedia(SocialMedia socialMedia)
        {
            var userId = User.GetUserId();
            var profile = _unitOfWork.Profile.GetUser(userId);
            
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
                    _unitOfWork.Profile.UpdateSocial(userSocialMedia, profile.Id);
                else
                    _unitOfWork.Profile.AddSocial(userSocialMedia);
                _unitOfWork.Complete();


                return RedirectToAction("UserInput");
            }

            return View(socialMedia);
        }
        [HttpPost]
        public IActionResult SaveLink(Link model)
        {
            var userId = User.GetUserId();
            model.UserId = User.GetUserId();
            var buttonStyle = _unitOfWork.Profile.GetActiveStyle(userId);

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
            if (buttonStyle != null)
            {
                model.ButtonStyle = buttonStyle.ButtonStyle;
            }
            if (model.Id == 0)
                _unitOfWork.Profile.AddLink(model);
            else
                _unitOfWork.Profile.UpdateLink(model);
            _unitOfWork.Complete();

            return RedirectToAction("UserInput");
        }
        [HttpPost]
        public IActionResult SaveLinkStyle(Link model)
        {
            var userId = User.GetUserId();
            var userLinks = _unitOfWork.Profile.GetLinks(userId);
            try
            {
                foreach (var link in userLinks)
                {
                    link.ButtonStyle = model.ButtonStyle;
                    _unitOfWork.Profile.UpdateLink(link);
                }
                _unitOfWork.Complete();
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
            var userId = User.GetUserId();
            var link = _unitOfWork.Profile.FindLink(linkId);
            var buttonStyle = _unitOfWork.Profile.GetActiveStyle(userId);
            var linkmodel = new Link
            {
                Id = linkId,
                Name = link.Name,
                Url = link.Url,
                UserId = link.UserId,
                IsVisible = link.IsVisible,
                ButtonStyle = buttonStyle.ButtonStyle,
            };

            return View("_UserLinks", linkmodel);
        }
        [HttpPost]
        public ActionResult DeleteLink(int id)
        {
            try
            {
                var userId = User.GetUserId();
                _unitOfWork.Profile.DeleteLink(id, userId);
                _unitOfWork.Complete();
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
                _unitOfWork.Profile.AddPhoto(model, profilePicture);
            else
            {
                var existingProfile = _unitOfWork.Profile.GetUser(userId);
                if (existingProfile != null)
                    model.ProfileImage = existingProfile.ProfileImage;
            }
            _unitOfWork.Complete();
        }
    }
}
