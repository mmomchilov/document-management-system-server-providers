//  ----------------------------------------------------------------------- 
//   <copyright file="DbContextInitialiser.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Initialisation
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Common.Initialisation;
    using Kernel.DependencyResolver;

    public class DbContextInitialiser : Initialiser
    {
        public override byte Order => 0;

        /// <summary>
        ///     Performs initialisation.
        /// </summary>
        /// <param name="dependencyResolver"></param>
        /// <returns></returns>
        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            //Register EF context manually. Multiple context may exist to pick from
            dependencyResolver.RegisterType<GlasswallDbContext>(Lifetime.Transient);

            //Register connection string dependency in ConnectionDefinitionParser(..., Func<PropertyInfo, string> configNameConverter)
            dependencyResolver.RegisterFactory<Func<PropertyInfo, string>>(() => x => x.Name, Lifetime.Transient);
            ////Register connection string dependency in ConnectionDefinitionParser(Func<NameValueCollection> connectionPropertiesFactory, ...)
            //dependencyResolver.RegisterFactory<Func<NameValueCollection>>(() => this.GetConnecitonStringAttributes, Lifetime.Transient);
            return Task.FromResult<object>(null);
        }
    }
}