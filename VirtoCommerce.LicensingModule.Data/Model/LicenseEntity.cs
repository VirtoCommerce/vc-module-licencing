using System;
using Omu.ValueInjecter;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Model
{
    public class LicenseEntity : AuditableEntity
    {
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Type { get; set; }

        public virtual License ToModel(License license)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            license.InjectFrom(this);

            //License.LicenseStatus = EnumUtility.SafeParse<LicenseStatus>(this.Status, LicenseStatus.Active);
            return license;
        }

        public virtual LicenseEntity FromModel(License license, PrimaryKeyResolvingMap pkMap)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            pkMap.AddPair(license, this);

            this.InjectFrom(license);

            //this.Status = License.LicenseStatus.ToString();
            return this;
        }

        public virtual void Patch(LicenseEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            //target.CustomerName = this.CustomerName;
            //target.Number = this.Number;
            //target.Status = this.Status;
            //target.EndDate = this.EndDate;
        }
    }
}
