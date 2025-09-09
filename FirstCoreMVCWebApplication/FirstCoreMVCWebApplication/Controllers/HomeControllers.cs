using Microsoft.AspNetCore.Mvc;
namespace FirstCoreMVCWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "This is Index action from MVC Controller";
        }
    }
}