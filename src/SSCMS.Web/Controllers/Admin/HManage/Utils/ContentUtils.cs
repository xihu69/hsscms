using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.HManage.Utils
{
    public class ContentUtils
    {
        public static bool IsFileImpType(string extendName) {
            return StringUtils.EqualsIgnoreCase(extendName,".csv");
        }
    }
}
