//  ----------------------------------------------------------------------- 
//   <copyright file="ConstructionTests.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1.GlasswallDbContextTests
{
    using System;
    using Kernel.Data.ORM;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ConstructionTests
    {
        [Test]
        [Category(L1.EntityFramework)]
        public void Exception_thrown_when_dboptions_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new GlasswallDbContext(null, new Mock<IDbCustomConfiguration>().Object));
        }

        [Test]
        [Category(L1.EntityFramework)]
        public void Exception_thrown_when_configuration_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new GlasswallDbContext(new DbContextOptions<GlasswallDbContext>(), null));
        }

        [Test]
        [Category(L1.EntityFramework)]
        public void Fully_satified_constructor_does_not_throw()
        {
            Assert.DoesNotThrow(() =>
                new GlasswallDbContext(new DbContextOptions<GlasswallDbContext>(), new Mock<IDbCustomConfiguration>().Object));
        }
    }
}