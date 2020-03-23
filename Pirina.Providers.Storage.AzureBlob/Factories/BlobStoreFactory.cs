using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Storage;
using Glasswall.Providers.Storage.AzureBlob.Configuration;

namespace Glasswall.Providers.Storage.AzureBlob.Factories
{
    public class BlobStoreFactory : IStorageFactory<Guid>
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly Dictionary<string, IStorage<Guid>> _storages = new Dictionary<string, IStorage<Guid>>();

        public BlobStoreFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
        }

        public Task<IStorage<Guid>> GetStorage<T>(T connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (!(connection is string)) throw new ArgumentException("Connection is not a string", nameof(connection));

            var configuration = new BlobConfiguration(connection.ToString());

            if (_storages.ContainsKey(configuration.ConnectionString))
                return Task.FromResult(_storages[configuration.ConnectionString]);

            _dependencyResolver.RegisterFactory<IStorageConfiguration>(() => configuration, Lifetime.Transient);

            var store = _dependencyResolver.Resolve<IStorage<Guid>>();
            _storages.Add(configuration.ConnectionString, store);
            return Task.FromResult(store);
        }
    }
}
