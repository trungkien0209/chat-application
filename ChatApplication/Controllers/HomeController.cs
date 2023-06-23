using ChatApplication.Models;
using ChatApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;  
using System.Web.Mvc;

namespace ChatApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public ActionResult Chat()
        {

            return View();
        }

        [HttpPost]
        public ActionResult GetChatBox(int toUserId)
        {
            ChatBoxModel chatBoxModel = new AppService().GetChatBox(toUserId);

            return PartialView("~/Views/Partials/_ChatBox.cshtml", chatBoxModel);
        }

        [HttpPost]
        public ActionResult GetGroupChat(int toGroupId)
        {
            ChatGroupModel chatgroup = new AppService().GetChatGroup(toGroupId);
            return PartialView("~/Views/Partials/_ChatGroup.cshtml", chatgroup);
        }

        public ActionResult GetRoom()
        {
            DbChat db = new DbChat();
            List<Room> listRoom = db.Rooms.ToList();
            return PartialView("~/Views/Partials/_Room.cshtml", listRoom);
        }

        [HttpGet]
        public ActionResult AddRoom()
        {
            return View(new Room());
        }

        [HttpPost]
        public ActionResult AddRoom(Room room)
        {
            using(DbChat db = new DbChat())
            {
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Chat", "Home");
            }
            
        }
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(int toUserId, string message)
        {
            return Json(new AppService().SendMessage(toUserId, message));
        }

        [HttpPost]
        public ActionResult SendMessageGroup(int toGroupId, string message)
        {
            return Json(new AppService().SendMessageGroup(toGroupId, message));
        }

    }
}