//  ----------------------------------------------------------------------- 
//   <copyright file="CustomModelCacheKeyFactory.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.CustomServices
{
    using Kernel.Data.ORM;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    internal class CustomModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            return ((IDbContext) context).CustomConfiguration.ModelKey;
        }
    }
}