using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omu.ValueInjecter;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Model
{
    public class LicenseEntity : AuditableEntity
    {
        [Required]
        [StringLength(256)]
        public string CustomerEmail { get; set; }

        [Required]
        [StringLength(256)]
        public string CustomerName { get; set; }

        [Required]
        [Index("IX_ActivationCode", 1, IsUnique = true)]
        [StringLength(16)]
        public string ActivationCode { get; set; }

        [Required]
        [StringLength(64)]
        public string Signature { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [StringLength(32)]
        public string Type { get; set; }

        public virtual License ToModel(License license)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            license.InjectFrom(this);
            return license;
        }

        public virtual LicenseEntity FromModel(License license, PrimaryKeyResolvingMap pkMap)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            pkMap.AddPair(license, this);

            this.InjectFrom(license);

            return this;
        }

        public virtual void Patch(LicenseEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ActivationCode = ActivationCode;
            target.CustomerEmail = CustomerEmail;
            target.CustomerName = CustomerName;
            target.ExpirationDate = ExpirationDate;
            target.Signature = Signature;
            target.Type = Type;
        }
    }
}
