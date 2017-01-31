using System;
using System.Data.Entity;
using System.Linq;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.LicensingModule.Data.Repositories
{
    public class LicenseRepository : EFRepositoryBase, ILicenseRepository
    {
        public LicenseRepository()
        {
        }

        public LicenseRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenseEntity>().HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<LicenseEntity>().ToTable("License");

            base.OnModelCreating(modelBuilder);
        }

        #region ILicenseRepository
        public IQueryable<LicenseEntity> Licenses
        {
            get
            {
                return GetAsQueryable<LicenseEntity>();
            }
        }

        public LicenseEntity[] GetByIds(string[] ids)
        {
            return Licenses.Where(x => ids.Contains(x.Id)).ToArray();
        }

        public void RemoveByIds(string[] ids)
        {
            var entries = GetByIds(ids);
            foreach (var entry in entries)
            {
                Remove(entry);
            }
        }
        #endregion
    }
}
