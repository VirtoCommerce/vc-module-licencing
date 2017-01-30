using System.Linq;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Repositories
{
    public interface ILicenseRepository : IRepository
    {
        IQueryable<LicenseEntity> Licenses { get; }

        LicenseEntity[] GetLicensesByIds(string[] ids);
    }
}
