using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Kernel.Extensions;
using Glasswall.Kernel.Serialisation;
using Glasswall.Kernel.Storage;
using Glasswall.Providers.Storage.AzureBlob.Connection;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Glasswall.Providers.Storage.AzureBlob.Store
{
    //TODO: Add Second (endpoint) and retry logic
    public class BlobStore : IStorage<Guid>
    {
        private readonly IStorageConnection<CloudBlobContainer, Guid> _storageConnection;
        private readonly IJsonSerialiser _jsonSerialiser;
        private readonly IBlobSizeCalculator _blobSizeCalculator;

        public BlobStore(IStorageConnection<CloudBlobContainer, Guid> storageConnection, IJsonSerialiser jsonSerialiser, IBlobSizeCalculator blobSizeCalculator)
        {
            _storageConnection = storageConnection ?? throw new ArgumentNullException(nameof(storageConnection));
            _jsonSerialiser = jsonSerialiser ?? throw new ArgumentNullException(nameof(jsonSerialiser));
            _blobSizeCalculator = blobSizeCalculator ?? throw new ArgumentNullException(nameof(blobSizeCalculator));
        }

        public async Task AddAsync<TData>(TData data, Guid id, string key)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (id == Guid.Empty) throw new ArgumentException($"{nameof(id)} cannot be Empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} cannot be Null or Empty");

            using (var stream = await ConvertDataToStream(data))
            {
                var block = await GetBlockAsync(id, key,
                    await _blobSizeCalculator.GetBlockSize(stream.Length));
                await block.UploadFromStreamAsync(stream);
            }
        }

        public async Task AddAsync<TData>(TData data, Guid id, string key, string objectName)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (id == Guid.Empty) throw new ArgumentException($"{nameof(id)} cannot be Empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} cannot be Null or Empty");
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentException($"{nameof(objectName)} cannot be Null or Empty");

            var path = Path.Combine(objectName, key);
            await AddAsync(data, id, path);
        }

        public async Task<TData> GetAsync<TData>(Guid id, string key)
        {
            if (id == Guid.Empty) throw new ArgumentException($"{nameof(id)} cannot be Empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} cannot be Null or Empty");

            TData data;
            var block = await GetBlockAsync(id, key);
            using (var ms = new MemoryStream())
            {
                await block.DownloadToStreamAsync(ms);
                ms.Position = 0;
                data = await ConvertStreamToData<TData>(ms);
            }
            return data;
        }

        public async Task<TData> GetAsync<TData>(Guid id, string key, string objectName)
        {
            if (id == Guid.Empty) throw new ArgumentException($"{nameof(id)} cannot be Empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} cannot be Null or Empty");
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentException($"{nameof(objectName)} cannot be Null or Empty");

            var path = Path.Combine(objectName, key);
            return await GetAsync<TData>(id, path);
        }

        public async Task RemoveAsync(Guid id, string key)
        {
            if (id == Guid.Empty) throw new ArgumentException($"{nameof(id)} cannot be Empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} cannot be Null or Empty");

            var block = await GetBlockAsync(id, key);
            await block.DeleteIfExistsAsync();
        }

        public async Task RemoveAsync(Guid id, string key, string objectName)
        {
            if (id == Guid.Empty) throw new ArgumentException($"{nameof(id)} cannot be Empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentException($"{nameof(key)} cannot be Null or Empty");
            if (string.IsNullOrEmpty(objectName)) throw new ArgumentException($"{nameof(objectName)} cannot be Null or Empty");

            var path = Path.Combine(objectName, key);
            await RemoveAsync(id, path);
        }

        public async Task RemoveAllAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty) throw new ArgumentException($"{nameof(transactionId)} cannot be Empty");
            await _storageConnection.RemoveObjectAsync(transactionId);
        }

        private async Task<CloudBlockBlob> GetBlockAsync(Guid transactionId, string fileName)
        {
            var container = await _storageConnection.GetObjectAsync(transactionId);
            return container.GetBlockBlobReference(fileName);
        }

        private async Task<CloudBlockBlob> GetBlockAsync(Guid transactionId, string fileName, int size)
        {
            var block = await GetBlockAsync(transactionId, fileName);
            block.StreamWriteSizeInBytes = size;
            return block;
        }

        private async Task<Stream> ConvertDataToStream<TData>(TData data)
        {
            if (data.GetType().IsSubclassOf(typeof(Stream)))
                return data as Stream;

            var jsonString = await _jsonSerialiser.SerialiseToJson(data);
            return jsonString.ToStream(Encoding.UTF8);
        }

        private async Task<TData> ConvertStreamToData<TData>(Stream stream)
        {
            var obj = await _jsonSerialiser.Deserialise<TData>(stream);
            return obj;
        }
    }
}
