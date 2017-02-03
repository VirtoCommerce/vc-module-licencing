using VirtoCommerce.LicensingModule.Core.Model;

namespace VirtoCommerce.LicensingModule.Core.Events
{
    public class LicenseSignedEvent
    {
        public LicenseSignedEvent(License license, string clientIpAddress, bool isActivated)
        {
            License = license;
            ClientIpAddress = clientIpAddress;
            IsActivated = isActivated;
        }

        public License License { get; set; }
        public string ClientIpAddress { get; set; }
        public bool IsActivated { get; set; }
    }
}
