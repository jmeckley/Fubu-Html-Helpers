using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            object errors = null;
            TempData.TryGetValue("errors", out errors);
            ModelState.Merge(errors as ModelStateDictionary);

            object model = null;
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

                return RedirectToAction("Index");
            }

            return RedirectToAction("About");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}