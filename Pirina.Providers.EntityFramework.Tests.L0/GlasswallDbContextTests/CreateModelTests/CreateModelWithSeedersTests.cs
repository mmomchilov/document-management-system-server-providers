//  ----------------------------------------------------------------------- 
//   <copyright file="CreateModelWithSeedersTests.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1.GlasswallDbContextTests.CreateModelTests
{
    using System;
    using System.Collections.Generic;
    using Kernel.Data.ORM;
    using Kernel.Data.Tenancy;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using TestArtifacts;

    [TestFixture]
    public class CreateModelWithSeedersTests : DbTestBase
    {
        private Mock<ISeeder<ModelBuilder>> _seederMock1;
        private Mock<ISeeder<ModelBuilder>> _seederMock2;
        private Mock<ISeeder<ModelBuilder>> _seederMock3;
        private int _callOrder;
        private bool _calledInOrder;

        protected override void SetUpConfigutationMock()
        {
            ConfigurationMock.Setup(a => a.ModelKey).Returns("CreateModelWithSeedersTests");
            ConfigurationMock.Setup(a => a.TenantManager).Returns(() => () => TenantManagerMock.Object);
            _callOrder = 0;

            _calledInOrder = true;

            // Assert that the seeders are ordered by seeding order.
            _seederMock1 = new Mock<ISeeder<ModelBuilder>>();
            _seederMock1.Setup(a => a.SeedingOrder).Returns(1);
            _seederMock1.Setup(a => a.Seed(It.IsAny<ModelBuilder>())).Callback(() =>
            {
                _calledInOrder = ++_callOrder == 1 && _calledInOrder;
            });

            _seederMock2 = new Mock<ISeeder<ModelBuilder>>();
            _seederMock2.Setup(a => a.SeedingOrder).Returns(2);;
            _seederMock2.Setup(a => a.Seed(It.IsAny<ModelBuilder>())).Callback(() =>
            {
                _calledInOrder = ++_callOrder == 2 && _calledInOrder;
            });

            _seederMock3 = new Mock<ISeeder<ModelBuilder>>();
            _seederMock3.Setup(a => a.SeedingOrder).Returns(3);;
            _seederMock3.Setup(a => a.Seed(It.IsAny<ModelBuilder>())).Callback(() =>
            {
                _calledInOrder = ++_callOrder == 3 && _calledInOrder;
            });
            
            ConfigurationMock.Setup(a => a.Seeders).Returns(new List<ISeeder> { _seederMock2.Object, _seederMock3.Object, _seederMock1.Object });
        }

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            using (var context = new GlasswallDbContext(DbContextOptions, ConfigurationMock.Object))
            {
                context.Add(new TenantEntity());
            }
        }

        [Test]
        [Category(L1.EntityFramework)]
        public void seeders_are_ordered_by_seeding_order_before_seed_is_called()
        {
            Assert.That(_calledInOrder, Is.True, "Seeders have been called in the wrong order.");
        }

        [Test]
        [Category(L1.EntityFramework)]
        public void seed_is_called_on_all_supplied_seeders()
        {
            _seederMock1.Verify(seeder => seeder.Seed(It.IsAny<ModelBuilder>()), Times.Once);
            _seederMock2.Verify(seeder => seeder.Seed(It.IsAny<ModelBuilder>()), Times.Once);
            _seederMock3.Verify(seeder => seeder.Seed(It.IsAny<ModelBuilder>()), Times.Once);
        }
    }
}