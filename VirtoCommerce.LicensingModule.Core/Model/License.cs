using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Core.Model
{
    public class License : AuditableEntity
    {
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Type { get; set; }

    }
}
