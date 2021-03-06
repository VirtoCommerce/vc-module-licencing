﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.LicensingModule.Data.Services
{
    public class LicenseService : ServiceBase, ILicenseService
    {
        private readonly Func<ILicenseRepository> _licenseRepositoryFactory;
        private readonly IChangeLogService _changeLogService;
        private readonly IEventPublisher<LicenseSignedEvent> _licenseSignedEventPublisher;
        private readonly IEventPublisher<LicenseChangedEvent> _licenseChangedEventPublisher;
        private readonly ISettingsManager _settingsManager;

        public LicenseService(Func<ILicenseRepository> licenseRepositoryFactory, IChangeLogService changeLogService, ISettingsManager settingsManager, IEventPublisher<LicenseSignedEvent> licenseSignedEventPublisher, IEventPublisher<LicenseChangedEvent> licenseChangedEventPublisher)
        {
            _licenseRepositoryFactory = licenseRepositoryFactory;
            _changeLogService = changeLogService;
            _settingsManager = settingsManager;
            _licenseSignedEventPublisher = licenseSignedEventPublisher;
            _licenseChangedEventPublisher = licenseChangedEventPublisher;
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
                        entity.ActivationCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
                    }

                    var originalEntity = existingEntities.FirstOrDefault(x => x.Id == entity.Id);
                    var originalLicense = originalEntity != null ? originalEntity.ToModel(AbstractTypeFactory<License>.TryCreateInstance()) : entity;
                    //Raise event
                    var changeEvent = new LicenseChangedEvent(originalEntity == null ? EntryState.Added : EntryState.Modified, originalLicense, entity);
                    _licenseChangedEventPublisher.Publish(changeEvent);

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

        public string GetSignedLicense(string code, string clientIpAddress, bool isActivated)
        {
            string result = null;

            var licenseEntity = GetByCode(code);
            if (licenseEntity != null)
            {
                var license = new
                {
                    licenseEntity.Type,
                    licenseEntity.CustomerName,
                    licenseEntity.CustomerEmail,
                    licenseEntity.ExpirationDate,
                };

                var licenseString = JsonConvert.SerializeObject(license);
                var signature = CreateSignature(licenseString);

                result = string.Join("\r\n", licenseString, signature);

                //Raise event
                var activateEvent = new LicenseSignedEvent(licenseEntity.ToModel(AbstractTypeFactory<License>.TryCreateInstance()), clientIpAddress, isActivated);
                _licenseSignedEventPublisher.Publish(activateEvent);
            }

            return result;
        }


        private LicenseEntity GetByCode(string code)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                return repository.Licenses
                    .FirstOrDefault(x => x.ActivationCode == code);
            }
        }

        private string CreateSignature(string data)
        {
            var hashAlgorithmName = HashAlgorithmName.SHA256.Name;

            using (var hashAlgorithm = HashAlgorithm.Create(hashAlgorithmName))
            using (var rsa = new RSACryptoServiceProvider())
            {
                // TODO: Store private key in a more secure storage, for example in Azure Key Vault
                var privateKey = _settingsManager.GetValue("Licensing.SignaturePrivateKey", string.Empty);
                if (!string.IsNullOrEmpty(privateKey))
                {
                    rsa.FromXmlString(privateKey);
                }

                var signatureFormatter = new RSAPKCS1SignatureFormatter(rsa);
                signatureFormatter.SetHashAlgorithm(hashAlgorithmName);

                var dataBytes = Encoding.UTF8.GetBytes(data);
                var dataHash = hashAlgorithm?.ComputeHash(dataBytes) ?? new byte[0];
                var signatureBytes = signatureFormatter.CreateSignature(dataHash);
                var signature = Convert.ToBase64String(signatureBytes);

                return signature;
            }
        }
    }
}
