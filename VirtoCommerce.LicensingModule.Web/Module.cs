using System;
using Microsoft.Practices.Unity;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Observers;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.LicensingModule.Data.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.LicensingModule.Web
{
    public class Module : ModuleBase
    {
        private const string _connectionStringName = "VirtoCommerce";
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            base.Initialize();

            _container.RegisterType<IEventPublisher<LicenseSignedEvent>, EventPublisher<LicenseSignedEvent>>();
            _container.RegisterType<IEventPublisher<LicenseChangedEvent>, EventPublisher<LicenseChangedEvent>>();
            //log License activations and changes
            _container.RegisterType<IObserver<LicenseSignedEvent>, LogLicenseEventsObserver>("LogLicenseActivationsObserver");
            _container.RegisterType<IObserver<LicenseChangedEvent>, LogLicenseEventsObserver>("LogLicenseChangesObserver");

            _container.RegisterType<ILicenseRepository>(new InjectionFactory(c => new LicenseRepository(_connectionStringName, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));
            _container.RegisterType<ILicenseService, LicenseService>();
        }
    }
}
