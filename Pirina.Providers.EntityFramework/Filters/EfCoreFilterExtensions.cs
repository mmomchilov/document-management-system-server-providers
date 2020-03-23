//  ----------------------------------------------------------------------- 
//   <copyright file="EfCoreFilterExtensions.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Filters
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Kernel.Data.Tenancy;
    using Microsoft.EntityFrameworkCore;

    public static class EfCoreFilterExtensions
    {
        public static void SetTenantFilter(this ModelBuilder modelBuilder, Type entityType, Guid tenantId)
        {

            SetSoftDeleteFilterMethod.MakeGenericMethod(entityType)
                .Invoke(null, new object[] { modelBuilder, tenantId });
        }

        static readonly MethodInfo SetSoftDeleteFilterMethod = typeof(EfCoreFilterExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetTenantFilter");

        public static void SetTenantFilter<TEntity>(this ModelBuilder modelBuilder, Guid tenantId) 
            where TEntity : BaseTenantModel
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => x.TenantId == tenantId && !x.IsDeleted);
        }
    }
}