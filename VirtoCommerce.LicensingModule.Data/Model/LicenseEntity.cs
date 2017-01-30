using Omu.ValueInjecter;
using System;
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

        public virtual License ToModel(License License)
        {
            if (License == null)
                throw new NullReferenceException("License");

            License.InjectFrom(this);

            //License.LicenseStatus = EnumUtility.SafeParse<LicenseStatus>(this.Status, LicenseStatus.Active);
            return License;
        }

        public virtual LicenseEntity FromModel(License License, PrimaryKeyResolvingMap pkMap)
        {
            if (License == null)
                throw new NullReferenceException("License");

            pkMap.AddPair(License, this);

            this.InjectFrom(License);

            //this.Status = License.LicenseStatus.ToString();
            return this;
        }

        public virtual void Patch(LicenseEntity target)
        {
            if (target == null)
                throw new NullReferenceException("target");

            //target.CustomerName = this.CustomerName;
            //target.Number = this.Number;
            //target.Status = this.Status;
            //target.EndDate = this.EndDate;
        }
    }
}
