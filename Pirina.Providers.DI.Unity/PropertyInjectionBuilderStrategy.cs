using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Builder;
using Unity.Builder.Strategy;
using TypeExtensions = Pirina.Kernel.Reflection.Extensions.TypeExtensions;

namespace Pirina.Providers.DI.Unity
{
    internal class PropertyInjectionBuilderStrategy : BuilderStrategy
	{
		private readonly IUnityContainer unityContainer;
		private static readonly ConcurrentDictionary<Tuple<Type, PropertyInfo>, Func<object, object>> GetDelegateCache;
		private static readonly ConcurrentDictionary<Tuple<Type, PropertyInfo>, Action<object, object>> SetDelegateCache;

		/// <summary>
		/// Initializes the <see cref="PropertyInjectionBuilderStrategy"/> class.
		/// </summary>
		static PropertyInjectionBuilderStrategy()
		{
			PropertyInjectionBuilderStrategy.GetDelegateCache = new ConcurrentDictionary<Tuple<Type, PropertyInfo>, Func<object, object>>();
			PropertyInjectionBuilderStrategy.SetDelegateCache = new ConcurrentDictionary<Tuple<Type, PropertyInfo>, Action<object, object>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjectionBuilderStrategy"/> class.
		/// </summary>
		/// <param name="unityContainer">The unity container.</param>
		public PropertyInjectionBuilderStrategy(IUnityContainer unityContainer)
		{
			this.unityContainer = unityContainer;
		}

		/// <summary>
		/// Called during the chain of responsibility for a build operation. The
		/// PreBuildUp method is called when the chain is being executed in the
		/// forward direction.
		/// </summary>
		/// <param name="context">Context of the build operation.</param>
		public override void PreBuildUp(IBuilderContext context)
		{
			if (!context.BuildKey.Type.FullName.StartsWith("Microsoft.Practices"))
			{
				var properties = context.BuildKey.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(x => x.CanWrite && x.SetMethod.IsPublic);

				foreach (var property in properties)
				{
					if (unityContainer.IsRegistered(property.PropertyType)
					   && this.GetPropertyValue(context.Existing, property) == null)
					{
						var resolvedValue = unityContainer.Resolve(property.PropertyType);
						this.SetPropertyValue(context.Existing, property, resolvedValue);
					}
				}
			}
		}

		/// <summary>
		/// Gets the property value. Builds a property get accesssor expression if not found in the cache
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="propertyInfo">The property information.</param>
		/// <returns></returns>
		private object GetPropertyValue(object target, PropertyInfo propertyInfo)
		{
			var key = Tuple.Create(target.GetType(), propertyInfo);
			var getDelegate = PropertyInjectionBuilderStrategy.GetDelegateCache.GetOrAdd(key, TypeExtensions.GetInstancePropertyDelegate<object>(key.Item1, propertyInfo.Name));
			var value = getDelegate(target);
			return value;
		}

		/// <summary>
		/// Sets the property value. Builds a property set accessor if not found in the cache
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="propertyInfo">The property information.</param>
		/// <param name="valueToAssign">The value to assign.</param>
		private void SetPropertyValue(object target, PropertyInfo propertyInfo, object valueToAssign)
		{
			var key = Tuple.Create(target.GetType(), propertyInfo);
			var getDelegate = PropertyInjectionBuilderStrategy.SetDelegateCache.GetOrAdd(key, TypeExtensions.GetAssignPropertyDelegate(key.Item1, propertyInfo.Name));
			getDelegate(target, valueToAssign);
		}
	}
}