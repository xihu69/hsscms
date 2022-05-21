using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.HManage
{
    [Route(HBaseController.ApiPrefix+"/[controller]/[action]")]
    [ApiController]
   public abstract class HBaseController : ControllerBase
    {
        public const string ApiPrefix = "api/HManage";

    }
   public interface IDto
    {
    }
}
