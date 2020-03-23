using System;
using System.Collections.Generic;
using System.Linq;
using Pirina.Kernel.DependencyResolver;
using Unity;

namespace Pirina.Providers.DI.Unity
{
    public partial class UnityDependencyResolver : IDependencyResolver
	{
		#region Resolve

		/// <summary>
		/// Tries the resolve.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resolved">The resolved.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool TryResolve<T>(out T resolved)
		{
			object result;
			var canResolve = this.TryResolve(typeof(T), out result);
			resolved = (T)result;
			return canResolve;
		}

		/// <summary>
		/// Tries the resolve.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="resolved">The resolved.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool TryResolve(Type type, out object resolved)
		{
			resolved = null;
			try
			{
				resolved = this.ResolveInternal(type);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Resolves this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Resolve<T>()
		{
			return (T)this.Resolve(typeof(T));
		}

		/// <summary>
		/// Resolves the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public object Resolve(Type type)
		{
			try
			{
				return this.ResolveInternal(type);
			}
			catch (Exception ex)
			{
				//log here
				//LoggerManager.WriteExceptionToEventLog(ex);
				throw;
			}
		}

		/// <summary>
		/// Resolves all.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> ResolveAll<T>()
		{
			return this.ResolveAll(typeof(T))
				.Cast<T>()
				.ToList();
		}

		/// <summary>
		/// Resolves all.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public IEnumerable<object> ResolveAll(Type type)
		{
			try
			{
                this.CheckIsDisposed();

				var instances = container.ResolveAll(type)
					.ToList();

				if (this.container.IsRegistered(type, String.Empty))
				{
					var defaultInstance = this.container.Resolve(type, String.Empty);
					instances.Add(defaultInstance);
				}

				return instances;
			}
			catch (Exception ex)
			{
				// log here
				//LoggerManager.WriteExceptionToEventLog(ex);
				throw;
			}
		}

		#endregion

		#region private methods

		private object ResolveInternal(Type type)
		{
			this.CheckIsDisposed();

			object resolved = null;

			if (!this.TryToResolveAbstract(type, out resolved))
				resolved = this.container.Resolve(type, String.Empty);

			return resolved;
		}

        private bool TryToResolveAbstract(Type typeToResolve, out object result)
        {
            result = null;

            // if it's an interface or abstract class get all registration and return the single one. Throws an exception if no or multiple registrations found
            if (!typeToResolve.IsInterface && !typeToResolve.IsAbstract)
                return false;


            if (!this.Contains(typeToResolve) && !this.IsGenericRegister(typeToResolve))
                throw new InvalidOperationException(String.Format("No registration for abstract type or interface: {0} found.", typeToResolve.Name));

            result = this.GetDefaultRegistration(typeToResolve);

            var allRegistrations = this.container.ResolveAll(typeToResolve)
                .ToList();

            if (allRegistrations.Count > 0)
                throw new InvalidOperationException(String.Format("Multiple registrations for abstract type or interface: {0} found. Call ResolveAll to get all instances", typeToResolve.Name));

            return true;
        }

		private bool IsGenericRegister(Type typeToCheck)
		{
			if (!typeToCheck.IsGenericType)
				return false;
			var result = this.Contains(typeToCheck.GetGenericTypeDefinition());
			return result;
		}

		private object GetDefaultRegistration(Type type)
		{
			return this.container.Resolve(type, String.Empty);
		}

		private void CheckIsDisposed()
		{
			if (this.isDisposed)
				throw new ObjectDisposedException("The container has been disposed and cannot be used any longer. Call CreateChild method to obtain a new resolver");
		}

		#endregion

		#region dispose

		public void Dispose()
		{
			if (this.isDisposed)
				return;

			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing && this.container != null)
			{
				this.isDisposed = true;
				this.container.Dispose();
			}
		}

		#endregion
	}
}