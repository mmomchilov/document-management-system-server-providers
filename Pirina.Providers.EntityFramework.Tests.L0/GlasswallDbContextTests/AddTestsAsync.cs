//  ----------------------------------------------------------------------- 
//   <copyright file="AddTestsAsync.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1.GlasswallDbContextTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Kernel.Data.ORM;
    using NUnit.Framework;
    using TestArtifacts;

    [TestFixture]
    public class AddTestsAsync : DbTestBase
    {
        [Test]
        [Category(L1.EntityFrameworkAsync)]
        public async Task Can_add_entity_to_the_database()
        {
            var entity = new NonTenantEntity();

            var configurationMockObject = ConfigurationMock.Object;

            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObject) as IDbContext)
            {
                context.Add(entity);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new GlasswallDbContext(DbContextOptions, configurationMockObject) as IDbContext)
            {
                var list = context.Set<NonTenantEntity>().ToList();

                Assert.That(list.Count, Is.EqualTo(1));
            }
        }
    }
}