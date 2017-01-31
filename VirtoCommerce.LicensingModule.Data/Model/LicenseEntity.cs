using Omu.ValueInjecter;
using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual License ToModel(License License)
        {
            if (License == null)
                throw new NullReferenceException("License");

            License.InjectFrom(this);
            return License;
        }

        public virtual LicenseEntity FromModel(License License, PrimaryKeyResolvingMap pkMap)
        {
            if (License == null)
                throw new NullReferenceException("License");

            pkMap.AddPair(License, this);

            this.InjectFrom(License);

            return this;
        }

        public virtual void Patch(LicenseEntity target)
        {
            if (target == null)
                throw new NullReferenceException("target");

            target.ActivationCode = ActivationCode;
            target.CustomerEmail = CustomerEmail;
            target.CustomerName = CustomerName;
            target.ExpirationDate = ExpirationDate;
            target.Signature = Signature;
            target.Type = Type;
        }
    }
}
