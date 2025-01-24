using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Errors;

namespace Shop_System.Controllers
{
    public class ErrorController : ApiBaseController
    {
        [HttpGet]
        public IActionResult Error(int code)
            => new ObjectResult(new ApiResponse(code));
    }
}
