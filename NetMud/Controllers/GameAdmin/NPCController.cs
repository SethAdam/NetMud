﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NetMud.Authentication;
using NetMud.Data.EntityBackingData;
using NetMud.Data.LookupData;
using NetMud.DataAccess;
using NetMud.DataAccess.Cache;
using NetMud.Models.Admin;
using System.Web;
using System.Web.Mvc;

namespace NetMud.Controllers.GameAdmin
{
    public class NPCController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public NPCController()
        {
        }

        public NPCController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ActionResult Index(string SearchTerms = "", int CurrentPageNumber = 1, int ItemsPerPage = 20)
        {
            var vModel = new ManageNPCDataViewModel(BackingDataCache.GetAll<NonPlayerCharacter>());
            vModel.authedUser = UserManager.FindById(User.Identity.GetUserId());

            vModel.CurrentPageNumber = CurrentPageNumber;
            vModel.ItemsPerPage = ItemsPerPage;
            vModel.SearchTerms = SearchTerms;

            return View("~/Views/GameAdmin/NPC/Index.cshtml", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(long ID, string authorize)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(authorize) || !ID.ToString().Equals(authorize))
                message = "You must check the proper authorize radio button first.";
            else
            {
                var authedUser = UserManager.FindById(User.Identity.GetUserId());

                var obj = BackingDataCache.Get<NonPlayerCharacter>(ID);

                if (obj == null)
                    message = "That does not exist";
                else if (obj.Remove())
                {
                    LoggingUtility.LogAdminCommandUsage("*WEB* - RemoveNPC[" + ID.ToString() + "]", authedUser.GameAccount.GlobalIdentityHandle);
                    message = "Delete Successful.";
                }
                else
                    message = "Error; Removal failed.";
            }

            return RedirectToAction("Index", new { Message = message });
        }

        [HttpGet]
        public ActionResult Add()
        {
            var vModel = new AddEditNPCDataViewModel();
            vModel.authedUser = UserManager.FindById(User.Identity.GetUserId());
            vModel.ValidRaces = BackingDataCache.GetAll<Race>();

            return View("~/Views/GameAdmin/NPC/Add.cshtml", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string Name, string SurName, string Gender, long raceId)
        {
            string message = string.Empty;
            var authedUser = UserManager.FindById(User.Identity.GetUserId());

            var newObj = new NonPlayerCharacter();
            newObj.Name = Name;
            newObj.SurName = SurName;
            newObj.Gender = Gender;
            var race = BackingDataCache.Get<Race>(raceId);

            if (race != null)
                newObj.RaceData = race;

            if (newObj.Create() == null)
                message = "Error; Creation failed.";
            else
            {
                LoggingUtility.LogAdminCommandUsage("*WEB* - AddNPCData[" + newObj.ID.ToString() + "]", authedUser.GameAccount.GlobalIdentityHandle);
                message = "Creation Successful.";
            }

            return RedirectToAction("Index", new { Message = message });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            string message = string.Empty;
            var vModel = new AddEditNPCDataViewModel();
            vModel.authedUser = UserManager.FindById(User.Identity.GetUserId());
            vModel.ValidRaces = BackingDataCache.GetAll<Race>();

            var obj = BackingDataCache.Get<NonPlayerCharacter>(id);

            if (obj == null)
            {
                message = "That does not exist";
                return RedirectToAction("Index", new { Message = message });
            }

            vModel.DataObject = obj;
            vModel.Name = obj.Name;
            vModel.Gender = obj.Gender;
            vModel.SurName = obj.SurName;

            return View("~/Views/GameAdmin/NPC/Edit.cshtml", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Name, string SurName, string Gender, long raceId, int id)
        {
            string message = string.Empty;
            var authedUser = UserManager.FindById(User.Identity.GetUserId());

            var obj = BackingDataCache.Get<NonPlayerCharacter>(id);
            if (obj == null)
            {
                message = "That does not exist";
                return RedirectToAction("Index", new { Message = message });
            }

            obj.Name = Name;
            obj.SurName = SurName;
            obj.Gender = Gender;
            var race = BackingDataCache.Get<Race>(raceId);

            if (race != null)
                obj.RaceData = race;

            if (obj.Save())
            {
                LoggingUtility.LogAdminCommandUsage("*WEB* - EditNPCData[" + obj.ID.ToString() + "]", authedUser.GameAccount.GlobalIdentityHandle);
                message = "Edit Successful.";
            }
            else
                message = "Error; Edit failed.";

            return RedirectToAction("Index", new { Message = message });
        }
    }
}