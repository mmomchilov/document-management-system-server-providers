using System;
using System.Linq;
using System.Threading.Tasks;
using Pirina.Kernel.DependencyResolver;
using Pirina.Providers.Microsoft.Dependency.Tests.L0.MockData;
using Pirina.Providers.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Pirina.Providers.Microsoft.Dependency.Tests.L0
{
    [TestFixture]
    public class FactoryRegistrationTest
    {
        [Test]
        public void Register_factory_gineric_service_implementor_explicit_singleton()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterFactory<ITestInterface>(() => instance, Lifetime.Singleton);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Singleton, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.IsNotNull(registration.ImplementationFactory);
            Assert.IsNull(registration.ImplementationType);
            Assert.IsNull(registration.ImplementationInstance);
        }

        [Test]
        public void Register_factory_gineric_service_implementor_explicit_transient()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterFactory<ITestInterface>(() => instance, Lifetime.Transient);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Transient, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.IsNotNull(registration.ImplementationFactory);
            Assert.IsNull(registration.ImplementationType);
            Assert.IsNull(registration.ImplementationInstance);
        }

        [Test]
        public void Register_factory_gineric_service_implementor_explicit_scoped()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterFactory<ITestInterface>(() => instance, Lifetime.PerThread);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Scoped, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.IsNotNull(registration.ImplementationFactory);
            Assert.IsNull(registration.ImplementationType);
            Assert.IsNull(registration.ImplementationInstance);
        }

        [Test]
        public async Task Register_factory_gineric_service_implementor_explicit_after_initialised_trhows_invalid_operation_exception()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterFactory<ITestInterface>(() => instance, Lifetime.Transient);
            await resolver.Initialise();

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => resolver.RegisterFactory<ITestInterface>(() => instance, Lifetime.Transient));
        }

        [Test]
        public async Task Resolve()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            var instance = new Derived();
            //ACT
            resolver.RegisterFactory<ITestInterface>(() => instance, Lifetime.Singleton);
            await resolver.Initialise();
            var service = resolver.Resolve<ITestInterface>();
            //ASSERT
            Assert.AreSame(instance, service);
        }
    }
}