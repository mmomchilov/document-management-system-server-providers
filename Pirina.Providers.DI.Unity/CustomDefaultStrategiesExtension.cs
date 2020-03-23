using Unity.Builder;
using Unity.Extension;

namespace Pirina.Providers.DI.Unity
{
    internal class CustomDefaultStrategiesExtension : UnityContainerExtension
	{
		/// <summary>
		/// Initial the container with this extension's functionality.
		/// </summary>
		/// <remarks>
		/// When overridden in a derived class, this method will modify the given
		/// context by adding strategies, policies, etc. to
		/// install it's functions into the container.
		/// </remarks>
		protected override void Initialize()
		{
			//overrides default unity constructor policy. The custom one select the shortest cnstrulctor unlike the default.
			//Uncommend this line out if you want to override the default behaviour
			//PolicyListExtensions.SetDefault<IConstructorSelectorPolicy>(this.Context.Policies, new CustomConstructorSelectorPolicy());

			//Registers a custom property strategy allowing property ingection by default. The implementation injects all public
			//properties if register in the container. Comment this line out if you want this functionality off
			Context.Strategies.Add(new PropertyInjectionBuilderStrategy(Container), UnityBuildStage.Initialization);
		}
	}
}