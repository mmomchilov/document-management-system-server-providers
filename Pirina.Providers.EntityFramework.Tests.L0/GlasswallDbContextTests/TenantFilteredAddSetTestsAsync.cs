//  ----------------------------------------------------------------------- 
//   <copyright file="TenantFilteredAddSetTestsAsync.cs" company="Glasswall Solutions Ltd.">
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

    [TestFixture]
    public class TenantFilteredAddSetTestsAsync : DbTestBase
    {
        private IList<TenantEntity> _listTenant1;
        private IList<TenantEntity> _listTenant2;
        private readonly Guid _objTenantId = Guid.NewGuid();
        private readonly Guid _objTenant2Id = Guid.NewGuid();

        private Mock<IDbCustomConfiguration> _configurationMockTenant2;
        private Mock<ITenantManager> _tenantManagerMockTenant2;

        private ITenantManager Tenant2Manager()
        {
            return _tenantManagerMockTenant2.Object;
        }

        protected override void SetUpMocks()
        {
            ConfigurationMock.Setup(a => a.ModelKey).Returns("KeyAsync");
            ConfigurationMock.Setup(a => a.TenantManager).Returns(() => () => TenantManagerMock.Object);
            ConfigurationMock.Setup(a => a.Seeders).Returns(new List<ISeeder>());

            _configurationMockTenant2 = new Mock<IDbCustomConfiguration>();
            _tenantManagerMockTenant2 = new Mock<ITenantManager>();

            _configurationMockTenant2.Setup(a => a.ModelKey).Returns("Key2Async");
            _configurationMockTenant2.Setup(a => a.TenantManager).Returns((Func<ITenantManager>)Tenant2Manager);
            _configurationMockTenant2.Setup(a => a.Seeders).Returns(new List<ISeeder>());

            TenantManagerMock.Setup(a => a.ResolveTenant()).Returns(_objTenantId);
            TenantManagerMock.Setup(a => a.AssignTenantId(It.IsAny<BaseTenantModel>())).Callback<BaseTenantModel>(a => SetTenantId(a, _objTenantId));

            _tenantManagerMockTenant2.Setup(a => a.ResolveTenant()).Returns(_objTenant2Id);
            _tenantManagerMockTenant2.Setup(a => a.AssignTenantId(It.IsAny<BaseTenantModel>())).Callback<BaseTenantModel>(a => SetTenantId(a, _objTenant2Id));
        }

        [OneTimeSetUp]
        public async Task TestSetUp()
        {
            var configurationMockObjectTenant1 = ConfigurationMock.Object;
            var configurationMockObjectTenant2 = _configurationMockTenant2.Object;

            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObjectTenant1) as IDbContext)
            {
                context.Add(new TenantEntity());
                context.Add(new TenantEntity());
                context.Add(new TenantEntity());
                context.Add(new TenantEntity());
                await context.SaveChangesAsync();
            }

            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObjectTenant2) as IDbContext)
            {
                context.Add(new TenantEntity());
                context.Add(new TenantEntity());
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObjectTenant1) as IDbContext)
            {
                _listTenant1 = context.Set<TenantEntity>().ToList();
            }

            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObjectTenant2) as IDbContext)
            {
                _listTenant2 = context.Set<TenantEntity>().ToList();
            }
        }

        [Test]
        [Category(L1.EntityFrameworkAsync)]
        public void Additions_of_entity_for_tenant_one_are_only_returned_for_tenant_one()
        {
            Assert.That(_listTenant1.Count, Is.EqualTo(4));
        }

        [Test]
        [Category(L1.EntityFrameworkAsync)]
        public void Additions_of_entity_for_tenant_two_are_only_returned_for_tenant_two()
        {
            Assert.That(_listTenant2.Count, Is.EqualTo(2));
        }
    }
}