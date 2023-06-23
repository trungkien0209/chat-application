using ChatApplication.Hubs;
using ChatApplication.Models;
using ChatApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;


namespace ChatApplication.Controllers
{
    [Authorize]

    public class UserController : Controller
    {

        // GET: User
        DbChat db = new DbChat();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Chat", "User");
            }
            return View(new LoginData());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginData loginData)
        {
            if (ModelState.IsValid)
            {
                int userId;
                if (new AppService().Login(loginData, out userId))
                {
                    FormsAuthentication.RedirectFromLoginPage(userId.ToString(), loginData.RememberMe);
                    return RedirectToAction("Chat", "Home");
                }
                else
                {
                    ViewBag.LoginError = "Email or password is incorrect!!";
                }
            }


            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            User usr = new User();
            return View(usr);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            catch
            {
                ViewData["Register"] = "Please enter full information!";
                return View();
            }

        }
        [HttpGet]
        public ActionResult EditProfile(int? id)
        {

            if (id != null)
            {
                var userById = db.Users.Find(id);
                return View(userById);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditProfile(User acc, HttpPostedFileBase photo)
        {
            int userId = int.Parse(User.Identity.Name);

            var user = db.Users.Find(userId);

            user.FullName = acc.FullName;
            user.Email = acc.Email;
            user.Birthday = acc.Birthday;

            if (photo != null && photo.ContentLength > 0)
            {
                Account cloudinaryAccount = new Account(
                    "dpkxwb7kr",
                    "888134633648832",
                    "nEjQGGRNlkkr9tChncNEZASxtYQ"
                );

                Cloudinary cloudinary = new Cloudinary(cloudinaryAccount);

                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var deleteImage = new DeletionParams(user.Photo);
                    cloudinary.Destroy(deleteImage);
                }

                var uploadImage = new ImageUploadParams()
                {
                    File = new FileDescription(photo.FileName, photo.InputStream),
                    Folder = "chatapp23",
                    PublicId = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")
                };

                var uploadResult = cloudinary.Upload(uploadImage);

                user.Photo = uploadResult.PublicId;
            }

            db.SaveChanges();

            return RedirectToAction("Chat", "Home");

        }

        public ActionResult Logout()
        {
            int userId = int.Parse(User.Identity.Name);
            new AppService().RemoveAllUserConnections(userId);
            ChatHub.OfflineUser(userId);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}