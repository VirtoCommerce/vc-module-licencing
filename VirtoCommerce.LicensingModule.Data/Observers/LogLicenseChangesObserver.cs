using System;
using System.Collections.Generic;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Observers
{
    public class LogLicenseChangesObserver : IObserver<LicenseChangeEvent>
    {
        private readonly IChangeLogService _changeLogService;

        public LogLicenseChangesObserver(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

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
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseChangesResources.NameChanged, original.CustomerName, modified.CustomerName));
                }
                if (original.CustomerEmail != modified.CustomerEmail)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseChangesResources.EmailChanged, original.CustomerEmail, modified.CustomerEmail));
                }
                if (original.ExpirationDate != modified.ExpirationDate)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseChangesResources.ExpirationDateChanged, original.ExpirationDate, modified.ExpirationDate));
                }
                if (original.Type != modified.Type)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseChangesResources.TypeChanged, original.Type, modified.Type));
                }
                if (original.ActivationCode != modified.ActivationCode)
                {
                    operationLogs.Add(GetLogRecord(modified.Id, LogLicenseChangesResources.ActivationCodeChanged, original.ActivationCode, modified.ActivationCode));
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
