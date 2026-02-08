using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/plans")]
[ApiController]
public class PlansController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAllPlans()
    {
        return Ok("Here are all plans: Gold, Silver...");
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreatePlan()
    {
        return Ok("Plan Created Successfully by Admin!");
    }
}