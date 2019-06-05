using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
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
        [CheckPermission(Permission = PredefinedPermissions.Read)]
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

        [HttpGet]
        [Route("download/{activationCode}")]
        [ResponseType(typeof(HttpResponseMessage))]
        [CheckPermission(Permission = PredefinedPermissions.Issue)]
        public HttpResponseMessage Download(string activationCode)
        {
            return GetSignedLicense(activationCode, false);
        }

        [HttpGet]
        [Route("activate/{activationCode}")]
        [ResponseType(typeof(HttpResponseMessage))]
        [AllowAnonymous]
        public HttpResponseMessage Activate(string activationCode)
        {
            return GetSignedLicense(activationCode, true);
        }


        private HttpResponseMessage GetSignedLicense(string activationCode, bool isActivated)
        {
            var clientIp = GetClientIpAddress(Request);
            var signedLicense = _licenseService.GetSignedLicense(activationCode, clientIp, isActivated);

            if (!string.IsNullOrEmpty(signedLicense))
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(signedLicense)) };
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "VirtoCommerce.lic" };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        private static string GetClientIpAddress(HttpRequestMessage requestMessage)
        {
            var request = (requestMessage.Properties["MS_HttpContext"] as HttpContextWrapper)?.Request;
            return request?.ServerVariables["HTTP_X_FORWARDED_FOR"]?.Split(',').FirstOrDefault() ?? request?.ServerVariables["REMOTE_ADDR"] ?? request?.UserHostAddress;
        }
    }
}
