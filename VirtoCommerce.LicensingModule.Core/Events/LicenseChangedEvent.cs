using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Core.Events
{
    public class LicenseChangedEvent
    {
        public LicenseChangedEvent(EntryState state, License originalLicense, License modifiedLicense)
        {
            State = state;
            OriginalLicense = originalLicense;
            ModifiedLicense = modifiedLicense;
        }

        public EntryState State { get; set; }
        public License OriginalLicense { get; set; }
        public License ModifiedLicense { get; set; }
    }
}
