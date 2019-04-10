using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTemplateExtended.Controllers
{
  [ApiController]
  [Route("api")]
  public class HomeController : ControllerBase
  {
    public string Index() => "AspNetCoreTemplateExtended API root endpoint";

    [Authorize]
    [HttpGet("restricted")]
    public string Restricted() => "You are authorized";
  }
}