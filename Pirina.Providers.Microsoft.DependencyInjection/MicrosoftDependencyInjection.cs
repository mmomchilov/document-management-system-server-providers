using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pirina.Kernel.DependencyResolver;
using Microsoft.Extensions.DependencyInjection;

namespace Pirina.Providers.Microsoft.DependencyInjection
{
    public class MicrosoftDependencyInjection : IDependencyResolver
    {
        private readonly ICollection<ServiceDescriptor> _serviceCollection;
        private IServiceProvider _serviceProvider;
        private bool _initialised;
        public MicrosoftDependencyInjection() : this(new ServiceCollection())
        {
        }

        public MicrosoftDependencyInjection(IServiceCollection serviceCollection)
        {
            this._serviceCollection = serviceCollection;
        }

        private MicrosoftDependencyInjection(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this._initialised = true;
        }

        public bool Contains<T>()
        {
            return this.Contains(typeof(T));
        }

        public bool Contains(Type type)
        {
            return this._serviceCollection.Any(x => x.ServiceType == type);
        }

        public IDependencyResolver CreateChildContainer()
        {
            this.EnsureInitialised();
            var scoped = this._serviceProvider.CreateScope();
            return new MicrosoftDependencyInjection((IServiceProvider)scoped);
        }

        public void Dispose()
        {
            var disposable = this._serviceProvider as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        public IDependencyResolver RegisterFactory<T>(Func<T> factory, Lifetime lifetime)
        {
            this.EnsureNotInitialised();
            var serviceLifetime = GetServiceLifetime(lifetime);
            var descriptor = new ServiceDescriptor(typeof(T), new Func<IServiceProvider, object>(_ => factory()), serviceLifetime);
            this._serviceCollection.Add(descriptor);
            return this;
        }

        public IDependencyResolver RegisterFactory<T>(Func<Type, T> factory, Lifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IDependencyResolver RegisterFactory(Type type, Func<Type, object> factory, Lifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IDependencyResolver RegisterInstance(Type t, string name, object instance, Lifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IDependencyResolver RegisterInstance<T>(string name, T instance, Lifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IDependencyResolver RegisterInstance<T>(T instance, Lifetime lifetime)
        {
            return this.RegisterInstance(typeof(T), instance, lifetime);
        }

        public IDependencyResolver RegisterInstance(Type service, object implementorInstance, Lifetime lifetime)
        {
            this.EnsureNotInitialised();

            if (lifetime != Lifetime.Singleton)
                throw new InvalidOperationException(String.Format("Life time must be {0}. It was: {1}", nameof(Lifetime.Singleton), lifetime));
            var descriptor = new ServiceDescriptor(service, implementorInstance);
            this._serviceCollection.Add(descriptor);
            return this;
        }

        public IDependencyResolver RegisterInstance<TService>(object implementorInstance, Lifetime lifetime)
        {
            return this.RegisterInstance(typeof(TService), implementorInstance, lifetime);
        }

        public IDependencyResolver RegisterNamedType(Type type, string name, Lifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public IDependencyResolver RegisterType<T>(Lifetime lifetime)
        {
            return this.RegisterType(typeof(T), lifetime);
        }

        public IDependencyResolver RegisterType(Type type, Lifetime lifetime)
        {
            throw new NotImplementedException();
            this.RegisterInheritance(type, lifetime);
            return this;
        }

        public IDependencyResolver RegisterType(Type service, Type implementor, Lifetime lifetime)
        {
            this.EnsureNotInitialised();
            var serviceLifetime = GetServiceLifetime(lifetime);
            var descriptor = new ServiceDescriptor(service, implementor, serviceLifetime);
            this._serviceCollection.Add(descriptor);
            return this;
        }

        public IDependencyResolver RegisterType<TService>(Type implementor, Lifetime lifetime)
        {
            return this.RegisterType(typeof(TService), implementor, lifetime);
        }

        public IDependencyResolver RegisterType<TService, TImplementor>(Lifetime lifetime)
        {
            return this.RegisterType(typeof(TService), typeof(TImplementor), lifetime);
        }

        public T Resolve<T>()
        {
            this.EnsureInitialised();
            return this._serviceProvider.GetService<T>();
        }

        public object Resolve(Type type)
        {
            this.EnsureInitialised();
            return this._serviceProvider.GetService(type);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            this.EnsureInitialised();
            return this._serviceProvider.GetServices<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            this.EnsureInitialised();
            return this._serviceProvider.GetServices(type);
        }

        public bool TryResolve<T>(out T resolved)
        {
            throw new NotImplementedException();
        }

        public bool TryResolve(Type type, out object resolved)
        {
            throw new NotImplementedException();
        }

        public Task Initialise()
        {
            if (this._serviceProvider == null)
                this._serviceProvider = ((ServiceCollection)this._serviceCollection).BuildServiceProvider();
            this._initialised = true;
            return Task.CompletedTask;
        }

        private void EnsureInitialised()
        {
            if (this._initialised)
                return;

            throw new InvalidOperationException("Container must be initialised before used. Call Initialise to build the container.");
        }

        private void EnsureNotInitialised()
        {
            if (!this._initialised)
                return;

            throw new InvalidOperationException("Container has been initialised. Once built registration is not allowed.");
        }

        private ServiceLifetime GetServiceLifetime(Lifetime lifeTime)
        {
            switch (lifeTime)
            {
                case Lifetime.Singleton:
                    return ServiceLifetime.Singleton;
                case Lifetime.Transient:
                    return ServiceLifetime.Transient;
                case Lifetime.PerThread:
                    return ServiceLifetime.Scoped;
            }

            throw new InvalidOperationException(String.Format("Unrecognised life time value: {0}", lifeTime));
        }

        private void RegisterInheritance(Type type, Lifetime lifetime)
        {
            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                this.RegisterBaseOrInterface(i, type, lifetime);
            }

            var baseType = type.BaseType;
            while (baseType != null)
            {
                this.RegisterBaseOrInterface(baseType, type, lifetime);
                baseType = baseType.BaseType;
            }
        }

        private void RegisterBaseOrInterface(Type from, Type to, Lifetime lifetime)
        {
            var lifeManger = this.GetServiceLifetime(lifetime);

            this.RegisterType(from, to, lifetime);
            
            this.RegisterGenericDefinition(from, to, lifetime);
        }

        private void RegisterGenericDefinition(Type from, Type to, Lifetime lifetime)
        {
            if (!from.IsGenericType)
                return;
            var genericDefinition = from.GetGenericTypeDefinition();
            
            this.RegisterType(genericDefinition, to, lifetime);
        }
    }
}
