using System;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.LicensingModule.Data.Services
{
    public class LicenseService : ServiceBase, ILicenseService
    {
        private readonly Func<ILicenseRepository> _licenseRepositoryFactory;

        public LicenseService(Func<ILicenseRepository> licenseRepositoryFactory)
        {
            _licenseRepositoryFactory = licenseRepositoryFactory;
        }

        public GenericSearchResult<License> Search(LicenseSearchCriteria criteria)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                var query = repository.Licenses;

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<License>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                var retVal = new GenericSearchResult<License>
                {
                    TotalCount = query.Count(),
                    Results = query.Skip(criteria.Skip)
                                    .Take(criteria.Take)
                                    .ToArray()
                                    .Select(x => x.ToModel(AbstractTypeFactory<License>.TryCreateInstance()))
                                    .ToArray()
                };
                return retVal;
            }
        }

        public void SaveChanges(License[] licenses)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _licenseRepositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existingEntities = repository.GetLicensesByIds(licenses.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var entity in licenses)
                {
                    var sourcePlanEntity = AbstractTypeFactory<LicenseEntity>.TryCreateInstance();
                    if (sourcePlanEntity != null)
                    {
                        sourcePlanEntity = sourcePlanEntity.FromModel(entity, pkMap);
                        var targetPlanEntity = existingEntities.FirstOrDefault(x => x.Id == entity.Id);
                        if (targetPlanEntity != null)
                        {
                            changeTracker.Attach(targetPlanEntity);
                            sourcePlanEntity.Patch(targetPlanEntity);
                        }
                        else
                        {
                            repository.Add(sourcePlanEntity);
                        }
                    }
                }

                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }
    }
}
