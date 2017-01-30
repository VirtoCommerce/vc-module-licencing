using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.LicensingModule.Core.Model;

namespace VirtoCommerce.LicensingModule.Core.Services
{
    public interface ILicenseService
    {
        GenericSearchResult<License> Search(LicenseSearchCriteria criteria);

        void SaveChanges(License[] licenses);
    }
}
