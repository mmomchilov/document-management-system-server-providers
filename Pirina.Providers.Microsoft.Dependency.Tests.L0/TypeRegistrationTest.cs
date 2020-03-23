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
    public class TypeRegistrationTest
    {
        [Test]
        public void RegisterType_non_gineric_service_implementor_explicit_transient()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Transient);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Transient, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.AreEqual(typeof(Derived), registration.ImplementationType);
            Assert.IsNull(registration.ImplementationInstance);
            Assert.IsNull(registration.ImplementationFactory);
        }

        [Test]
        public void RegisterType_non_gineric_service_implementor_explicit_singlenton()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Singleton);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Singleton, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.AreEqual(typeof(Derived), registration.ImplementationType);
            Assert.IsNull(registration.ImplementationInstance);
            Assert.IsNull(registration.ImplementationFactory);
        }

        [Test]
        public void RegisterType_non_gineric_service_implementor_explicit_scoped()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.PerThread);
            var registration = serviceCollection.Single();
            //ASSERT
            Assert.AreEqual(ServiceLifetime.Scoped, registration.Lifetime);
            Assert.AreEqual(typeof(ITestInterface), registration.ServiceType);
            Assert.AreEqual(typeof(Derived), registration.ImplementationType);
            Assert.IsNull(registration.ImplementationInstance);
            Assert.IsNull(registration.ImplementationFactory);
        }

        [Test]
        public async Task RegisterType_non_gineric_service_implementor_explicit_after_initialised_trhows_invalid_operation_exception()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Transient);
            await resolver.Initialise();

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Transient));
        }

        [Test]
        public async Task Resolve_transient()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Transient);
            await resolver.Initialise();
            var service = resolver.Resolve<ITestInterface>();
            var service1 = resolver.Resolve<ITestInterface>();
            //ASSERT
            Assert.AreNotSame(service, service1);
        }

        [Test]
        public async Task Resolve_singleton()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);

            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Singleton);
            await resolver.Initialise();
            var service = resolver.Resolve<ITestInterface>();
            var service1 = resolver.Resolve<ITestInterface>();
            //ASSERT
            Assert.AreSame(service, service1);
        }

        [Test]
        public async Task Resolve_scoped_resolved_from_different_scope()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);

            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.PerThread);
            await resolver.Initialise();
            var serviceFromParent = resolver.Resolve<ITestInterface>();
            var scopedResolver = resolver.CreateChildContainer();
            var scopedService = scopedResolver.Resolve<ITestInterface>();
            var scopedResolver1 = resolver.CreateChildContainer();
            var scopedService1 = scopedResolver1.Resolve<ITestInterface>();
            //
            //ASSERT
            Assert.AreNotSame(scopedService, scopedService1);
        }

        [Test]
        public async Task Resolve_scoped_resolved_from_same_scope()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);

            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.PerThread);
            await resolver.Initialise();
            var serviceFromParent = resolver.Resolve<ITestInterface>();
            var scopedResolver = resolver.CreateChildContainer();
            var scopedService = scopedResolver.Resolve<ITestInterface>();
            
            var scopedService1 = scopedResolver.Resolve<ITestInterface>();
            //
            //ASSERT
            Assert.AreSame(scopedService, scopedService1);
        }

        [Test]
        public void Scoped_disposed()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.PerThread);
            resolver.Initialise();
            var serviceFromParent = resolver.Resolve<ITestInterface>();
            var child = resolver.CreateChildContainer();
            var scopedService = child.Resolve<ITestInterface>();
            child.Dispose();

            //ASSERT
            Assert.IsNotNull(serviceFromParent);
            Assert.IsNotNull(scopedService);
            Assert.Throws<ObjectDisposedException>(() => child.Resolve<ITestInterface>());
        }

        [Test]
        public void Scoped_disposed1()
        {
            //ARRANGE
            var serviceCollection = new ServiceCollection();
            var resolver = new MicrosoftDependencyInjection(serviceCollection);
            //ACT
            resolver.RegisterType(typeof(ITestInterface), typeof(Derived), Lifetime.Transient);
            resolver.Initialise();
            var serviceFromParent = resolver.Resolve<ITestInterface>();
            var child = resolver.CreateChildContainer();
            var scopedService = child.Resolve<ITestInterface>();
            child.Dispose();

            //ASSERT
            Assert.IsNotNull(serviceFromParent);
            Assert.IsNotNull(scopedService);
            Assert.AreNotSame(serviceFromParent, scopedService);
            Assert.Throws<ObjectDisposedException>(() => child.Resolve<ITestInterface>());
        }
    }
}