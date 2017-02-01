using System;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.LicensingModule.Data.Services
{
    public class LicenseService : ServiceBase, ILicenseService
    {
        private readonly Func<ILicenseRepository> _licenseRepositoryFactory;
        private readonly IChangeLogService _changeLogService;
        private readonly IEventPublisher<LicenseChangeEvent> _eventPublisher;

        public LicenseService(Func<ILicenseRepository> licenseRepositoryFactory, IChangeLogService changeLogService, IEventPublisher<LicenseChangeEvent> eventPublisher)
        {
            _licenseRepositoryFactory = licenseRepositoryFactory;
            _changeLogService = changeLogService;
            _eventPublisher = eventPublisher;
        }

        public GenericSearchResult<License> Search(LicenseSearchCriteria criteria)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                var query = repository.Licenses;

                if (criteria.Keyword != null)
                {
                    query = query.Where(x => x.CustomerName.Contains(criteria.Keyword));
                }

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

        public License[] GetByIds(string[] ids)
        {
            License[] result;

            using (var repository = _licenseRepositoryFactory())
            {
                result = repository.GetByIds(ids)
                    .Select(x =>
                    {
                        var retVal = x.ToModel(AbstractTypeFactory<License>.TryCreateInstance());
                        //Load change log by separate request
                        _changeLogService.LoadChangeLogs(retVal);
                        return retVal;
                    })
                    .ToArray();
            }

            return result;
        }

        public void SaveChanges(License[] licenses)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _licenseRepositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existingEntities = repository.GetByIds(licenses.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var entity in licenses)
                {
                    //ensure that ActivationCode is filled
                    if (string.IsNullOrEmpty(entity.ActivationCode))
                    {
                        entity.ActivationCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                    }

                    //regenerate the Signature
                    entity.Signature = Guid.NewGuid().ToString();

                    var originalEntity = existingEntities.FirstOrDefault(x => x.Id == entity.Id);
                    var originalLicense = originalEntity != null ? originalEntity.ToModel(AbstractTypeFactory<License>.TryCreateInstance()) : entity;
                    //Raise event
                    var changeEvent = new LicenseChangeEvent(originalEntity == null ? EntryState.Added : EntryState.Modified, originalLicense, entity);
                    _eventPublisher.Publish(changeEvent);

                    var sourceEntity = AbstractTypeFactory<LicenseEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(entity, pkMap);
                        var targetEntity = existingEntities.FirstOrDefault(x => x.Id == entity.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }
                }

                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public void Delete(string[] ids)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                repository.RemoveByIds(ids);
                CommitChanges(repository);
            }
        }
    }
}
