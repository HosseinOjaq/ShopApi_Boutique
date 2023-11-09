using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers.v1
{
    public class ProductRatingController : Controller
    {        
        public IActionResult Index()
        {
            return View();
        }
    }
}
