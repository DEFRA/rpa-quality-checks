using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RPA.QualityPortal.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    [HttpGet]
    public IActionResult AccessDenied() => StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to access this resource.");

    [HttpGet]
    public IActionResult AuthenticationFailed() => StatusCode(StatusCodes.Status401Unauthorized, "Sign-in failed. Contact support if this continues.");
}
