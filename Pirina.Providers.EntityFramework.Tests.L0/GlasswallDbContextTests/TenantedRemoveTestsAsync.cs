//  ----------------------------------------------------------------------- 
//   <copyright file="TenantedRemoveTestsAsync.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1.GlasswallDbContextTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kernel.Data.ORM;
    using Kernel.Data.Tenancy;
    using Moq;
    using NUnit.Framework;
    using TestArtifacts;

    public class TenantedRemoveTestsAsync : DbTestBase
    {
        private readonly Guid _objTenantId = Guid.NewGuid();
        private readonly Guid _objTenant2Id = Guid.NewGuid();
        private Mock<IDbCustomConfiguration> _configurationMockTenant2;
        private Mock<ITenantManager> _tenantManagerMockTenant2;

        protected override Guid GetGuid()
        {
            return _objTenantId;
        }
        private ITenantManager Tenant2Manager()
        {
            return _tenantManagerMockTenant2.Object;
        }

        protected override void SetUpTenantManagerMock()
        {
            base.SetUpTenantManagerMock();
            TenantManagerMock.Setup(a => a.AssignTenantId(It.IsAny<BaseTenantModel>())).Callback<BaseTenantModel>(a => SetTenantId(a, _objTenantId));

            _configurationMockTenant2 = new Mock<IDbCustomConfiguration>();
            _tenantManagerMockTenant2 = new Mock<ITenantManager>();

            _configurationMockTenant2.Setup(a => a.ModelKey).Returns("Key2");
            _configurationMockTenant2.Setup(a => a.TenantManager).Returns((Func<ITenantManager>)Tenant2Manager);
            _configurationMockTenant2.Setup(a => a.Seeders).Returns(new List<ISeeder>());

            _tenantManagerMockTenant2.Setup(a => a.ResolveTenant()).Returns(_objTenant2Id);
            _tenantManagerMockTenant2.Setup(a => a.AssignTenantId(It.IsAny<BaseTenantModel>())).Callback<BaseTenantModel>(a => SetTenantId(a, _objTenant2Id));
        }

        [Test]
        [Category(L1.EntityFrameworkAsync)]
        public async Task Can_remove_tenanted_entity_from_the_database()
        {
            var entity = new TenantEntity();

            var configurationMockObject = ConfigurationMock.Object;

            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObject) as IDbContext)
            {
                context.Add(entity);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObject) as IDbContext)
            {
                var list = context.Set<TenantEntity>().ToList();
                Assert.That(list.Count, Is.EqualTo(1));

                Assert.That(context.Remove(list.First()), Is.True);

                var result = await context.SaveChangesAsync();

                Assert.That(result, Is.EqualTo(1));

                list = context.Set<TenantEntity>().ToList();

                Assert.That(list.Count, Is.EqualTo(0));
            }
        }
    }
}