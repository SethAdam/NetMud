﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NetMud.Authentication;
using NetMud.Data.LookupData;
using NetMud.DataAccess;
using NetMud.DataAccess.Cache;
using NetMud.DataStructure.Base.Place;
using NetMud.DataStructure.Base.Supporting;
using NetMud.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetMud.Controllers.GameAdmin
{
    public class WorldController : Controller
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

        public WorldController()
        {
        }

        public WorldController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ActionResult Index(string SearchTerms = "", int CurrentPageNumber = 1, int ItemsPerPage = 20)
        {
            var vModel = new ManageWorldViewModel(BackingDataCache.GetAll<IWorld>());
            vModel.authedUser = UserManager.FindById(User.Identity.GetUserId());

            vModel.CurrentPageNumber = CurrentPageNumber;
            vModel.ItemsPerPage = ItemsPerPage;
            vModel.SearchTerms = SearchTerms;

            return View("~/Views/GameAdmin/World/Index.cshtml", vModel);
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

                var obj = BackingDataCache.Get<IWorld>(ID);

                if (obj == null)
                    message = "That does not exist";
                else if (obj.Remove())
                {
                    LoggingUtility.LogAdminCommandUsage("*WEB* - RemoveWorld[" + ID.ToString() + "]", authedUser.GameAccount.GlobalIdentityHandle);
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
            var vModel = new AddEditWorldViewModel();
            vModel.authedUser = UserManager.FindById(User.Identity.GetUserId());

            vModel.ValidMaterials = BackingDataCache.GetAll<IMaterial>();

            return View("~/Views/GameAdmin/World/Add.cshtml", vModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddEditWorldViewModel vModel)
        {
            string message = string.Empty;
            var authedUser = UserManager.FindById(User.Identity.GetUserId());
            var validData = true;

            var newObj = new World(vModel.Name);
            newObj.FullDiameter = vModel.FullDiameter;
            newObj.Topography = vModel.Topography;

            if (vModel.StratumName != null)
            {
                int stratumIndex = 0;
                var stratumList = new List<IStratum>();

                foreach (var stratumName in vModel.StratumName)
                {
                    if (!string.IsNullOrEmpty(stratumName))
                    {
                        if (vModel.Diameter.Count() <= stratumIndex || vModel.AmbientTemperatureRangeLow.Count() <= stratumIndex
                            || vModel.AmbientTemperatureRangeHigh.Count() <= stratumIndex || vModel.AmbientHumidityRangeLow.Count() <= stratumIndex
                            || vModel.AmbientHumidityRangeHigh.Count() <= stratumIndex)
                            break;

                        var currentDiameter = vModel.Diameter[stratumIndex];
                        var currentTempLow = vModel.AmbientTemperatureRangeLow[stratumIndex];
                        var currentTempHigh = vModel.AmbientTemperatureRangeHigh[stratumIndex];
                        var currentHumidLow = vModel.AmbientHumidityRangeLow[stratumIndex];
                        var currentHumidHigh = vModel.AmbientHumidityRangeHigh[stratumIndex];

                        if (currentDiameter > 0 && currentTempLow < currentTempHigh && currentHumidLow < currentHumidHigh)
                        {
                            var newStrat = new Stratum();
                            newStrat.AmbientHumidityRange = new Tuple<int, int>(currentHumidLow, currentHumidHigh);
                            newStrat.AmbientTemperatureRange = new Tuple<int, int>(currentTempLow, currentTempHigh);
                            newStrat.Diameter = currentDiameter;
                            newStrat.Name = stratumName;

                            //Do layers now

                            stratumList.Add(newStrat);
                        }
                    }

                    stratumIndex++;
                }

                newObj.Strata = new HashSet<IStratum>(stratumList);
            }

            /*

        //per layer
        public long[] LayerMaterials { get; set; }
        public int[] LowerDepths { get; set; }
        public int[] UpperDepths { get; set; }
            */

            if (validData)
            {
                if (newObj.Create() == null)
                    message = "Error; Creation failed.";
                else
                {
                    LoggingUtility.LogAdminCommandUsage("*WEB* - AddWorldData[" + newObj.ID.ToString() + "]", authedUser.GameAccount.GlobalIdentityHandle);
                    message = "Creation Successful.";
                }
            }

            return RedirectToAction("Index", new { Message = message });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            string message = string.Empty;
            var vModel = new AddEditWorldViewModel();
            vModel.authedUser = UserManager.FindById(User.Identity.GetUserId());

            vModel.ValidMaterials = BackingDataCache.GetAll<IMaterial>();

            var obj = BackingDataCache.Get<IWorld>(id);

            if (obj == null)
            {
                message = "That does not exist";
                return RedirectToAction("Index", new { Message = message });
            }

            vModel.DataObject = obj;
            vModel.Name = obj.Name;
            vModel.FullDiameter = obj.FullDiameter;
            vModel.Topography = obj.Topography;

            return View("~/Views/GameAdmin/World/Edit.cshtml", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AddEditWorldViewModel vModel)
        {
            string message = string.Empty;
            var authedUser = UserManager.FindById(User.Identity.GetUserId());
            var validData = true;

            var obj = BackingDataCache.Get<IWorld>(id);
            if (obj == null)
            {
                message = "That does not exist";
                return RedirectToAction("Index", new { Message = message });
            }

            obj.Name = vModel.Name;

            if (validData)
            {
                if (obj.Save())
                {
                    LoggingUtility.LogAdminCommandUsage("*WEB* - EditWorldData[" + obj.ID.ToString() + "]", authedUser.GameAccount.GlobalIdentityHandle);
                    message = "Edit Successful.";
                }
                else
                    message = "Error; Edit failed.";
            }

            return RedirectToAction("Index", new { Message = message });
        }
    }
}