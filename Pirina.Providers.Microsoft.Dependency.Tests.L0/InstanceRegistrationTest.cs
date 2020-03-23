using Glasswall.Kernel.DependencyResolver;
using System.Linq;
using Pirina.Providers.Microsoft.Dependency.Tests.L0.MockData;
using Pirina.Providers.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Pirina.Providers.Microsoft.Dependency.Tests.L0
{
    [TestFixture]
    public class InstanceRegistrationTest
    {
        [Test]
        public void RegisterInstance_non_gineric_service_implementor_explicit_singleton()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Singleton);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Singleton, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.AreSame(instance, registration.ImplementationInstance);
            Assert.IsNull(registration.ImplementationType);
            Assert.IsNull(registration.ImplementationFactory);
        }

        [Test]
        public void RegisterInstance_non_gineric_service_implementor_explicit_transient_trows_invalid_operation_exception()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Transient));
        }

        [Test]
        public void RegisterInstance_non_gineric_service_implementor_explicit_scoped_throws_invalid_operation_exception()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.PerThread));
        }

        [Test]
        public async Task RegisterInstance_non_gineric_service_implementor_explicit_after_initialised_trhows_invalid_operation_exception()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Singleton);
            await resolver.Initialise();

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Singleton));
        }

        [Test]
        public async Task Resolve()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterInstance(typeof(ITestInterface), instance, Lifetime.Singleton);
            await resolver.Initialise();
            var service = resolver.Resolve<ITestInterface>();
            //ASSERT
            Assert.AreSame(instance, service);
        }
    }
}