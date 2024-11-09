using Microsoft.AspNetCore.Mvc;

namespace SCCD.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
       
    }
}