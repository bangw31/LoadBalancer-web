using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Refit;

namespace ECommerceSystem.GUI.Controllers
{
    //[Authorize]
    //[Route("User")]
    public class ChatController : Controller
    {
        [HttpGet("UserChat")]
        public IActionResult UserChat()
        {
            return PartialView("~/Views/User/UserChat.cshtml");
        }
    }
}
