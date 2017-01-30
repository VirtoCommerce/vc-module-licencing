using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Web.Security;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.LicensingModule.Web.Controllers.Api
{
    [RoutePrefix("api/licenses")]
    [CheckPermission(Permission = PredefinedPermissions.Read)]
    public class LicensingModuleController : ApiController
    {
        private readonly ILicenseService _licenseService;
        

        public LicensingModuleController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(GenericSearchResult<License>))]
        public IHttpActionResult SearchLicenses(LicenseSearchCriteria request)
        {
            if (request == null)
            {
                return BadRequest("request is null");
            }

            var searchResponse = _licenseService.Search(request);

            return Ok(searchResponse);
        }

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(License))]
        [CheckPermission(Permission = PredefinedPermissions.Create)]
        public IHttpActionResult CreateLicense(License license)
        {
            _licenseService.SaveChanges(new[] { license });
            return Ok(license);
        }

    }
}