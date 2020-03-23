//  ----------------------------------------------------------------------- 
//   <copyright file="SetTests.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1.GlasswallDbContextTests
{
    using System.Linq;
    using Kernel.Data.ORM;
    using Moq;
    using NUnit.Framework;
    using TestArtifacts;

    [TestFixture]
    public class TenantFilteredSetTests : DbTestBase
    {
        [Test]
        [Category(L1.EntityFramework)]
        public void Tenant_id_is_resolved_on_each_call_to_set()
        {
            // It is essential that the tenant id is resolved every time a set is called.

            using (var context = new GlasswallDbContext(DbContextOptions, ConfigurationMock.Object) as IDbContext)
            {
                context.Set<TenantEntity>().ToList();
                context.Set<TenantEntity>().ToList();
                context.Set<TenantEntity>().ToList();
                context.Set<TenantEntity>().ToList();
                context.Set<TenantEntity>().ToList();
            }

            TenantManagerMock.Verify(a => a.ResolveTenant(), Times.Exactly(5));
        }

        [Test]
        [Category(L1.EntityFramework)]
        public void Tenant_id_is_not_on_set_call_for_non_tenanted_entity()
        {
            using (var context = new GlasswallDbContext(DbContextOptions, ConfigurationMock.Object) as IDbContext)
            {
                context.Set<NonTenantEntity>().ToList();
            }

            TenantManagerMock.Verify(a => a.ResolveTenant(), Times.Never);
        }
    }
}