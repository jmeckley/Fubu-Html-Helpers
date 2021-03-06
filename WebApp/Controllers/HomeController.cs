﻿using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController 
        : Controller
    {
        public ActionResult Index()
        {
            object errors;
            TempData.TryGetValue("errors", out errors);
            ModelState.Merge(errors as ModelStateDictionary);

            object model;
            TempData.TryGetValue("model", out model);

            return View(model as LoginViewModel ?? new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                model.Password = "";
                model.ConfirmPassword = "";

                TempData.Add("model", model);
                TempData.Add("errors", ModelState);
            }

            return RedirectToAction("Index");
        }
    }
}