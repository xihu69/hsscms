using ELibrary.Common.BaseModel;
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
    public abstract class HBaseController<T>: HBaseController where T:IEntityBase
    {
        protected readonly IFreeSql freeSql;

        public HBaseController(IFreeSql freeSql) {
            this.freeSql = freeSql;
        }
      
       

    }
   public interface IDto
    {
    }
}
