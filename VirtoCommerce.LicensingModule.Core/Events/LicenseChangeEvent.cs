using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Core.Events
{
    public class LicenseChangeEvent
    {
        public LicenseChangeEvent(EntryState state, License originalLicense, License modifiedLicense)
        {
            ChangeState = state;
            OriginalLicense = originalLicense;
            ModifiedLicense = modifiedLicense;
        }

        public EntryState ChangeState { get; set; }
        public License OriginalLicense { get; set; }
        public License ModifiedLicense { get; set; }
    }
}
