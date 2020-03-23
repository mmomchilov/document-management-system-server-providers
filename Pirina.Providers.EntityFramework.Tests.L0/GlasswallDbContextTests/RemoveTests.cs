//  ----------------------------------------------------------------------- 
//   <copyright file="RemoveTests.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1.GlasswallDbContextTests
{
    using System.Linq;
    using Kernel.Data.ORM;
    using NUnit.Framework;
    using TestArtifacts;

    public class RemoveTests : DbTestBase
    {
        [Test]
        [Category(L1.EntityFramework)]
        public void Can_remove_entity_from_the_database()
        {
            var entity = new NonTenantEntity();

            var configurationMockObject = ConfigurationMock.Object;

            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObject) as IDbContext)
            {
                context.Add(entity);
                context.SaveChanges();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObject) as IDbContext)
            {
                var list = context.Set<NonTenantEntity>().ToList();
                Assert.That(list.Count, Is.EqualTo(1));

                Assert.That(context.Remove(list.First()), Is.True);

                context.SaveChanges();

                list = context.Set<NonTenantEntity>().ToList();

                Assert.That(list.Count, Is.EqualTo(0));
            }
        }
    }
}