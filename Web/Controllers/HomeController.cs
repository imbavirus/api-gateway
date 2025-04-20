using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Web;

public class HomeController : Controller
{
    public IActionResult Index() 
    {
        return View();
    }
};