using System;
using System.Collections.Generic;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Observers
{
    public class LogLicenseEventsObserver : IObserver<LicenseActivateEvent>, IObserver<LicenseChangeEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogLicenseEventsObserver(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        #region IObserver<LicenseActivateEvent>
        public void OnNext(LicenseActivateEvent value)
        {
            var log = GetLogRecord(value.License.Id, LogLicenseEventsResources.Activated, value.Ip);
            _changeLogService.SaveChanges(new[] { log });
        }
        #endregion

        #region IObserver<LicenseChangeEvent>
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(LicenseChangeEvent value)
        {
            var original = value.OriginalLicense;
            var modified = value.ModifiedLicense;

            if (value.ChangeState == EntryState.Modified)
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

        private static OperationLog GetLogRecord(string LicenseId, string template, params object[] parameters)
        {
            return new OperationLog
            {
                ObjectId = LicenseId,
                ObjectType = typeof(License).Name,
                OperationType = EntryState.Modified,
                Detail = string.Format(template, parameters)
            };
        }
    }
}
