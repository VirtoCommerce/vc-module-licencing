using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(License))]
        [CheckPermission(Permission = PredefinedPermissions.Read)]
        public IHttpActionResult GetLicenseById(string id)
        {
            var retVal = _licenseService.GetByIds(new[] { id }).FirstOrDefault();
            return Ok(retVal);
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

        [HttpPut]
        [Route("")]
        [ResponseType(typeof(License))]
        [CheckPermission(Permission = PredefinedPermissions.Update)]
        public IHttpActionResult UpdateLicense(License license)
        {
            _licenseService.SaveChanges(new[] { license });
            return Ok(license);
        }

        /// <summary>
        ///  Delete Licenses
        /// </summary>
        /// <param name="ids">Licenses' ids for delete</param>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.Delete)]
        public IHttpActionResult DeleteLicensesByIds([FromUri] string[] ids)
        {
            _licenseService.Delete(ids);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("getLicenseFile/{activationCode}")]
        [AllowAnonymous]
        [ResponseType(typeof(HttpResponseMessage))]
        public HttpResponseMessage GetLicenseFile(string activationCode)
        {
            //if (request == null)
            //{
            //    return BadRequest("request is null");
            //}

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("license file content"));
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "virtoCommerce.lic"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }
    }
}
