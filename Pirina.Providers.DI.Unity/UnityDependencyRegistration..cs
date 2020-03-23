using System;
using System.Threading;
using System.Threading.Tasks;
using Pirina.Kernel.DependencyResolver;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Pirina.Providers.DI.Unity
{
    public partial class UnityDependencyResolver : IDependencyResolver
	{
		#region static members

		private IUnityContainer container = new UnityContainer();
		//prevents concurrency issues when registering types and calling Contains method
		private static ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private bool isDisposed;

		/// <summary>
		/// Initializes the <see cref="UnityDependencyResolver"/> class.
		/// </summary>
		public UnityDependencyResolver()
		{
			//overwrites the default constructor starategy. Comment out the next line to enable default behaviour, which selects the longest constructor if
			//no constructor injecttion parameter is provided
			this.container.AddExtension(new CustomDefaultStrategiesExtension());
		}

		protected UnityDependencyResolver(IUnityContainer container)
			: this()
		{
			this.container = container;
		}

		#endregion

		#region register instance

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">The name.</param>
		/// <param name="instance">The instance.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		public IDependencyResolver RegisterInstance<T>(string name, T instance, Lifetime lifetime)
		{
			this.CheckIsDisposed();
			UnityDependencyResolver.readerWriterLock.EnterWriteLock();
			try
			{
				var lifeManager = this.GetLifeTimeManger(lifetime);
				this.container.RegisterInstance<T>(name, instance, lifeManager);
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitWriteLock();
			}
			return this;
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		public IDependencyResolver RegisterInstance<T>(T instance, Lifetime lifetime)
		{
			this.CheckIsDisposed();
			UnityDependencyResolver.readerWriterLock.EnterWriteLock();
			try
			{
				var lifeManager = this.GetLifeTimeManger(lifetime);
				this.container.RegisterInstance<T>(instance, lifeManager);
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitWriteLock();
			}
			return this;
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <param name="t">The t.</param>
		/// <param name="name">The name.</param>
		/// <param name="instance">The instance.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		public IDependencyResolver RegisterInstance(Type t, string name, object instance, Lifetime lifetime)
		{
			this.CheckIsDisposed();
			UnityDependencyResolver.readerWriterLock.EnterWriteLock();
			try
			{
				var lifeManager = this.GetLifeTimeManger(lifetime);
				this.container.RegisterInstance(t, name, instance, lifeManager);
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitWriteLock();
			}
			return this;
		}

        public IDependencyResolver RegisterInstance(Type service, object implementorInstance, Lifetime lifetime)
        {
            return this.RegisterInstance(service, null, implementorInstance, lifetime);
        }

        public IDependencyResolver RegisterInstance<TService>(object implementorInstance, Lifetime lifetime)
        {
            return this.RegisterInstance(typeof(TService), implementorInstance, lifetime);
        }
        #endregion

        #region register factory

        /// <summary>
        /// Registers the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <returns></returns>
        public IDependencyResolver RegisterFactory(Type type, Func<Type, object> factory, Lifetime lifetime)
		{
			this.CheckIsDisposed();
			UnityDependencyResolver.readerWriterLock.EnterWriteLock();
			try
			{
				this.container.RegisterType(type, new InjectionFactory((c, t, s) => factory(t)));
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitWriteLock();
			}
			return this;
		}

		/// <summary>
		/// Registers the specified name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">The name.</param>
		/// <param name="factory">The factory.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IDependencyResolver RegisterFactory<T>(Func<T> factory, Lifetime lifetime)
		{
			this.CheckIsDisposed();
			UnityDependencyResolver.readerWriterLock.EnterWriteLock();
			try
			{
				this.container.RegisterType<T>(new InjectionFactory(c => factory()));
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitWriteLock();
			}
			return this;
		}

		/// <summary>
		/// Registers the specified factory.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="factory">The factory.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		public IDependencyResolver RegisterFactory<T>(Func<Type, T> factory, Lifetime lifetime)
		{
			this.CheckIsDisposed();

			return this.RegisterFactory(typeof(T), (Func<Type, object>)(t => (T)factory(t)), lifetime);
		}

		#endregion

		#region register type

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		public IDependencyResolver RegisterType<T>(Lifetime lifetime)
		{
			return this.RegisterType(typeof(T), lifetime);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		public IDependencyResolver RegisterType(Type type, Lifetime lifetime)
		{
			this.CheckIsDisposed();
			UnityDependencyResolver.readerWriterLock.EnterWriteLock();
			try
			{
				var lifeManger = this.GetLifeTimeManger(lifetime);

				this.container.RegisterType(type, lifeManger);
				this.RegisterInheritance(type, lifetime);
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitWriteLock();
			}

			return this;
		}

		/// <summary>
		/// Registers the type of the named. NOTE: not implemented. Be aware of unity behaviour:
		/// If you have a parameter of type array that needs resolving only the name instances are considered. The default is omitted. Odd but it's
		/// how it works. If you need any class to be included in array you need to register as a named registration. See Unity documentation for more information
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name"></param>
		/// <param name="lifetime">The lifetime.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IDependencyResolver RegisterNamedType(Type type, string name, Lifetime lifetime)
		{
			throw new NotImplementedException();
		}

        public IDependencyResolver RegisterType(Type service, Type implementor, Lifetime lifetime)
        {
            UnityDependencyResolver.readerWriterLock.EnterWriteLock();
            try
            {
                var lifeManger = this.GetLifeTimeManger(lifetime);

                this.container.RegisterType(service, implementor, lifeManger);
                
            }
            finally
            {
                UnityDependencyResolver.readerWriterLock.ExitWriteLock();
            }

            return this;
        }

        public IDependencyResolver RegisterType<TService>(Type implementor, Lifetime lifetime)
        {
            UnityDependencyResolver.readerWriterLock.EnterWriteLock();
            try
            {
                var lifeManger = this.GetLifeTimeManger(lifetime);

                this.container.RegisterType(typeof(TService), implementor, lifeManger);

            }
            finally
            {
                UnityDependencyResolver.readerWriterLock.ExitWriteLock();
            }

            return this;
        }

        public IDependencyResolver RegisterType<TService, TImplementor>(Lifetime lifetime)
        {
            return this.RegisterType(typeof(TService), typeof(TImplementor), lifetime);
        }
        
        #endregion

        #region miscellaneous

        /// <summary>
        /// Creates the child container.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDependencyResolver CreateChildContainer()
		{
			return new UnityDependencyResolver(this.container.CreateChildContainer());
		}

		/// <summary>
		/// Determines whether [contains].
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool Contains<T>()
		{
			return this.Contains(typeof(T));
		}

		/// <summary>
		/// Determines whether [contains] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public bool Contains(Type type)
		{
			UnityDependencyResolver.readerWriterLock.EnterReadLock();
			try
			{
				return this.container.IsRegistered(type);
			}
			finally
			{
				UnityDependencyResolver.readerWriterLock.ExitReadLock();
			}
		}

        public Task Initialise()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private methods

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
			var lifeManger = this.GetLifeTimeManger(lifetime);

			if (!this.Contains(from))
				this.container.RegisterType(from, to, null, lifeManger);
			else
				this.container.RegisterType(from, to, Guid.NewGuid().ToString(), lifeManger);
			this.RegisterGenericDefinition(from, to, lifetime);
		}

		private void RegisterGenericDefinition(Type from, Type to, Lifetime lifetime)
		{
			if (!from.IsGenericType)
				return;
			var genericDefinition = from.GetGenericTypeDefinition();
			var lifeManger = this.GetLifeTimeManger(lifetime);
			this.container.RegisterType(genericDefinition, to, null, lifeManger);
		}

		private LifetimeManager GetLifeTimeManger(Lifetime lifeTime)
		{
			switch (lifeTime)
			{
				case Lifetime.Singleton:
					return new ContainerControlledLifetimeManager();
				case Lifetime.Transient:
					return new PerResolveLifetimeManager();
				case Lifetime.PerThread:
					return new PerThreadLifetimeManager();
			}

			throw new InvalidOperationException(String.Format("Unrecognised life time value: {0}", lifeTime));
		}
        
        #endregion
    }
}