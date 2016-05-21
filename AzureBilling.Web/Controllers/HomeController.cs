using System.Web.Mvc;

namespace AzureBilling.Web.Controllers
{
    /// <summary>
    /// Default Home Controller, initialize the angular landing page
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}