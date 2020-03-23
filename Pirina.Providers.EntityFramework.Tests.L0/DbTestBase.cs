//  ----------------------------------------------------------------------- 
//   <copyright file="DbTestBase.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Tests.L1
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Kernel.Data.ORM;
    using Kernel.Data.Tenancy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Debug;
    using Moq;
    using NUnit.Framework;

    public abstract class DbTestBase
    {
        protected Mock<IDbCustomConfiguration> ConfigurationMock;
        protected Mock<ITenantManager> TenantManagerMock;

        [OneTimeSetUp]
        public void SetUp()
        {
            ConfigurationMock = new Mock<IDbCustomConfiguration>();
            TenantManagerMock = new Mock<ITenantManager>();

            SetUpMocks();

            CreateDbOptions();
        }

        protected void SetTenantId<T>(T entity, Guid id) where T : BaseTenantModel
        {
            var prop = entity.GetType().GetProperty("TenantId",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            prop.SetValue(entity, id);
        }

        protected virtual void SetUpMocks()
        {
            SetUpConfigutationMock();
            SetUpTenantManagerMock();
        }

        protected virtual void SetUpTenantManagerMock()
        {
            TenantManagerMock.Setup(a => a.ResolveTenant()).Returns(GetGuid());
        }

        protected virtual Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        protected virtual void SetUpConfigutationMock()
        {
            ConfigurationMock.Setup(a => a.ModelKey).Returns(Guid.NewGuid().ToString());
            ConfigurationMock.Setup(a => a.TenantManager).Returns(() => () => TenantManagerMock.Object);
        }

        protected virtual void CreateDbOptions()
        {
            DbContextOptions = new DbContextOptionsBuilder<GlasswallDbContext>()
                .UseLoggerFactory(MyLoggerFactory)
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        protected static readonly LoggerFactory MyLoggerFactory
            =  new LoggerFactory(new[] { 
                new DebugLoggerProvider() 
            });

        protected DbContextOptions<GlasswallDbContext> DbContextOptions;

        [OneTimeTearDown]
        protected virtual void Teardown()
        {
            DbContextOptions = null;
            ConfigurationMock = null;
            TenantManagerMock = null;
        }
    }
}