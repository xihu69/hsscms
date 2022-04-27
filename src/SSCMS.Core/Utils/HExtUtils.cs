using SSCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSCMS.Core.Utils
{
    internal class HExtUtils
    {
        /// <summary>
        /// 追加一个内容id参数_scId
        /// </summary>
        /// <param name="contentCurrent"></param>
        /// <param name="re"></param>
        /// <returns></returns>
        public static string scidAppend(Content contentCurrent, string url)
        {
            if (url != null && url.IndexOf('?') > 0)
            {
                url = $"{url}&_scId={contentCurrent.SiteId}_{contentCurrent.Id}";
            }
            else
            {
                url = $"{url}?_scId={contentCurrent.SiteId}_{contentCurrent.Id}";
            }

            return url;
        }
    }
}
