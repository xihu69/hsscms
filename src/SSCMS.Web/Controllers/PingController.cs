using ELibrary.Utils;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Services;
using System.IO;
namespace SSCMS.Web.Controllers
{
    [Route("api/ping")]
    public partial class PingController : ControllerBase
    {
        private const string Route = "";
        private const string RouteIp = "ip";
        private const string RouteStatus = "status";

        private readonly ISettingsManager _settingsManager;

        public PingController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }
        [HttpGet, Route("p")]
        public IActionResult PdfPage(string path,int pageNum)
        {
            var referer = Request.Headers["Referer"];
            System.Console.WriteLine(referer);
            //是否存在
            //存在：返回
            //不存在：创建
            //返回

             var finfo = new FileInfo(Path.Join(Directory.GetCurrentDirectory(), @"wwwroot", path));

            if (!finfo.Exists)
                return NotFound();
            if (finfo.Length < 4 * 1024 * 1024)
                return File(path, "application/pdf");
            //var pageNum = 1;
            var pageSize = 20;
            var blockNum = (pageNum-1) / pageSize + 1;
            if (!System.IO.Directory.Exists(finfo + "_ls"))
            {
                PdfHandler.PegingFirstAsyn(finfo.FullName);
                if (blockNum != 1)
                    return null;
            }
            var blockPath = $"wwwroot/{path}_ls/{blockNum}.pdf";
            //PhysicalFile
            var newpath= Path.Join(Directory.GetCurrentDirectory(), @"wwwroot", path+"_ls",blockNum+".pdf");
            return PhysicalFile(newpath, "application/pdf");
        }

        public class StatusResult
        {
            public string Name { get; set; }
            public string Env { get; set; }
            public bool Containerized { get; set; }
            public string Version { get; set; }
            public bool IsDatabaseWorks { get; set; }
            public bool IsRedisWorks { get; set; }
        }
    }
}
