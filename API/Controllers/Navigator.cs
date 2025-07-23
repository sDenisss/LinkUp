using Microsoft.AspNetCore.Mvc;

namespace LinkUp.API;

[ApiController]
[Route("/")]
public class Navigator : ControllerBase
{
    [HttpGet("home")]
    public IActionResult GetHome()
    {
        return Redirect("/home.html");
    }

    [HttpGet("chat")]
    public IActionResult GetChat()
    {
        return Redirect("/chat.html");
    }

    [HttpGet("registry")]
    public IActionResult GetRegiistry()
    {
        return Redirect("/registry.html");
    }
    
    [HttpGet("login")]
    public IActionResult GetLogin()
    {
        return Redirect("/login.html");
    }

    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Redirect("/profile.html");
    }

    [HttpGet("profileChat")]
    public IActionResult GetProfileChat()
    {
        return Redirect("/profileChat.html");
    }


}
