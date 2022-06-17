﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            await _createManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
