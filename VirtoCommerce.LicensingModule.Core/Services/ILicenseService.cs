using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.LicensingModule.Core.Model;

namespace VirtoCommerce.LicensingModule.Core.Services
{
    public interface ILicenseService
    {
        GenericSearchResult<License> Search(LicenseSearchCriteria criteria);
        License[] GetByIds(string[] ids);
        void SaveChanges(License[] licenses);
        void Delete(string[] ids);
        string GetSignedLicense(string code, string clientIp);
    }
}
