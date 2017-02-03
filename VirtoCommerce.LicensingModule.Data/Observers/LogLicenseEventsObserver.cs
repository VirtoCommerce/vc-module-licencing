using System;
using System.Collections.Generic;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Observers
{
    public class LogLicenseEventsObserver : IObserver<LicenseSignedEvent>, IObserver<LicenseChangedEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogLicenseEventsObserver(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        #region IObserver<LicenseActivateEvent>
        public void OnNext(LicenseSignedEvent value)
        {
            var template = value.IsActivated ? LogLicenseEventsResources.Activated : LogLicenseEventsResources.Downloaded;
            var log = GetLogRecord(value.License.Id, template, value.ClientIpAddress);
            _changeLogService.SaveChanges(log);
        }
        #endregion

        #region IObserver<LicenseChangeEvent>
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(LicenseChangedEvent value)
        {
            var original = value.OriginalLicense;
            var modified = value.ModifiedLicense;

            if (value.State == EntryState.Modified)
            {
                var operationLogs = new List<OperationLog>();

                if (original.CustomerName != modified.CustomerName)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseEventsResources.NameChanged, original.CustomerName, modified.CustomerName));
                }
                if (original.CustomerEmail != modified.CustomerEmail)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseEventsResources.EmailChanged, original.CustomerEmail, modified.CustomerEmail));
                }
                if (original.ExpirationDate != modified.ExpirationDate)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseEventsResources.ExpirationDateChanged, original.ExpirationDate, modified.ExpirationDate));
                }
                if (original.Type != modified.Type)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseEventsResources.TypeChanged, original.Type, modified.Type));
                }
                if (original.ActivationCode != modified.ActivationCode)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseEventsResources.ActivationCodeChanged, original.ActivationCode, modified.ActivationCode));
                }

                _changeLogService.SaveChanges(operationLogs.ToArray());
            }
        }
        #endregion

        private static OperationLog GetLogRecord(string licenseId, string template, params object[] parameters)
        {
            return new OperationLog
            {
                ObjectId = licenseId,
                ObjectType = typeof(License).Name,
                OperationType = EntryState.Modified,
                Detail = string.Format(template, parameters)
            };
        }
    }
}
