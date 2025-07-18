using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LinkUp.API;

[ApiController]
[Route("api/[controller]")] 
// [Route("/")]
public class ProfileController : ControllerBase
{
    [HttpPost]
    public IActionResult RegisterUser()
    {
        return new RedirectToPageResult("/Shit"); // Перенаправляет на Razor Page
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok($"Your ID is {userId}");
    }

}
