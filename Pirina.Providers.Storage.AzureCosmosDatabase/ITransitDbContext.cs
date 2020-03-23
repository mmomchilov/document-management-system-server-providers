using System.Linq;
using Pirina.Kernel.Data;

namespace Pirina.Providers.Storage.AzureCosmosDatabase
{
    public interface ITransitDbContext
    {
        /// <summary>
        /// Sets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> Set<T>() where T : BaseTransactionModel;

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        T Add<T>(T item) where T : BaseTransactionModel;

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        bool Remove<T>(T item) where T : BaseTransactionModel;
    }
}