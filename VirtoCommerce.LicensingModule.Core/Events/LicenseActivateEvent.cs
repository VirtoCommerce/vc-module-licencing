using VirtoCommerce.LicensingModule.Core.Model;

namespace VirtoCommerce.LicensingModule.Core.Events
{
    public class LicenseActivateEvent
    {
        public LicenseActivateEvent(License license, string clientIp)
        {
            License = license;
            Ip = clientIp;
        }

        public License License { get; set; }
        public string Ip { get; set; }
    }
}
