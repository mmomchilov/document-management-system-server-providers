using System.Linq;
using Pirina.Kernel.DependencyResolver;
using Pirina.Providers.Microsoft.Dependency.Tests.L0.MockData;
using Pirina.Providers.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Pirina.Providers.Microsoft.Dependency.Tests.L0
{
    [TestFixture]
    public class ContainsTest
    {
        [Test]
        public void Contains_generic()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Singleton);
            var registration = serviceCollection.Single();
            var result = resolver.Contains<ITestInterface>();
            //ASSERT
            Assert.True(result);
        }

        [Test]
        public void Contains_non_generic()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Singleton);
            var registration = serviceCollection.Single();
            var result = resolver.Contains(typeof(ITestInterface));
            //ASSERT
            Assert.True(result);
        }
    }
}