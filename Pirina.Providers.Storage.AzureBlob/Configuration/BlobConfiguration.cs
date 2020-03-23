using System;
using Glasswall.Kernel.Storage;

namespace Glasswall.Providers.Storage.AzureBlob.Configuration
{
    public class BlobConfiguration : IStorageConfiguration
    {
        public string ConnectionString { get; }
        public string ObjectName { get; }
        public string Key { get; }

        public BlobConfiguration(string connectionString, string objectName = null, string key = null)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException($"{nameof(connectionString)} cannot be Null or Empty");

            ConnectionString = connectionString;
            ObjectName = objectName;
            Key = key;
        }
    }
}
