using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Pirina.MemoryCacheProvider
{
    public class MemoryCacheRuntimeImplementor : ICacheProvider
    {
        private static MemoryCache _memoryCache;

        static MemoryCacheRuntimeImplementor()
        {
            MemoryCacheRuntimeImplementor._memoryCache = new MemoryCache(new MemoryDistributedCacheOptions());
        }
        /// <summary>
        /// Occurs when [written to].
        /// </summary>
        public event EventHandler WrittenTo;

        /// <summary>
        /// Occurs when [read from].
        /// </summary>
        public event EventHandler ReadFrom;

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public object Get(string key)
        {
            key = this.FormatKey(key);
            object entry = null;
            try
            {
                entry = MemoryCacheRuntimeImplementor._memoryCache.Get(key);
                return entry ?? throw new InvalidOperationException(string.Format("Key '{0}' not found.", key));
            }
            finally
            {
                OnReadFrom(key, entry);
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// </exception>
        public T Get<T>(string key)
        {
            key = this.FormatKey(key);
            T entry = default(T);
            try
            {
                entry = MemoryCacheRuntimeImplementor._memoryCache.Get<T>(key);
                if (entry != null)
                    return entry;
                throw new InvalidOperationException(string.Format("Key '{0}' not found.", key));
            }
            finally
            {
                OnReadFrom(key, entry);
            }
        }

        public bool TryGet(string key, out object item)
        {
            key = this.FormatKey(key);
            return MemoryCacheRuntimeImplementor._memoryCache.TryGetValue(key, out item);
        }

        public bool TryGet<T>(string key, out T item)
        {
            key = this.FormatKey(key);
            return MemoryCacheRuntimeImplementor._memoryCache.TryGetValue<T>(key, out item);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<object, Task<T>> factory, CancellationToken cancellationToken = default(CancellationToken))
        {
            var read = true;
            key = this.FormatKey(key);
            var result = await MemoryCacheRuntimeImplementor._memoryCache.GetOrCreateAsync<T>(key, new Func<ICacheEntry, Task<T>>(e =>
            {
                read = false;
                e.Size = 1;
                return factory(e);
            }));
            if (read)
                this.OnReadFrom(key, result);
            else
                this.OnWrittenTo(key, result);
            return result;
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<object, Task<T>> factory, ICacheEntryOptions policy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var read = true;
            key = this.FormatKey(key);
            var result =  MemoryCacheRuntimeImplementor._memoryCache.GetOrCreateAsync<T>(key, new Func<ICacheEntry, Task<T>>(e =>
            {
                read = false;
                e.AbsoluteExpiration = policy.AbsoluteExpiration;
                e.SlidingExpiration = policy.SlidingExpiration;
                e.Size = 1;
                return factory(e);
            }));
            if (read)
                this.OnReadFrom(key, result);
            else
                this.OnWrittenTo(key, result);
            return result;
        }

        /// <summary>
        /// Inserts value with specified key or updates if it already exists
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Put(string key, object value)
        {
            var policy = new MemoryCacheItemPolicy();

            this.Put(key, value, policy);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="policy"></param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Put(string key, object value, ICacheEntryOptions policy)
        {
            var cachePolicy = policy as MemoryCacheEntryOptions;
            if (cachePolicy == null)
                throw new InvalidOperationException(String.Format("Expected type: {0}, but was: {1}", typeof(MemoryCacheEntryOptions).Name, policy.GetType().Name));

            key = this.FormatKey(key);

            MemoryCacheRuntimeImplementor._memoryCache.Set(key, value, cachePolicy);

            OnWrittenTo(key, value);
        }

        /// <summary>
        /// Deletes the value specified at key location.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object Delete(string key)
        {
            key = this.FormatKey(key);

            MemoryCacheRuntimeImplementor._memoryCache.Remove(key);

            return true;
        }

        /// <summary>
        /// Creates an entry at the given key, throws an exception if that entry already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Post(string key, object value)
        {
            var policy = new MemoryCacheItemPolicy();

            Post(key, value, policy);
        }

        /// <summary>
        /// Creates an entry at the given key, throws an exception if that entry already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="policy"></param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Post(string key, object value, ICacheEntryOptions policy)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        public void Initialise()
        {
        }

        /// <summary>
        /// Determines whether the cache contains anything with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string key)
        {
            key = this.FormatKey(key);
            object result;
            return MemoryCacheRuntimeImplementor._memoryCache.TryGetValue(key, out result);
        }

        /// <summary>
        /// Types the of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDictionary<string, T> TypeOf<T>()
        {
            throw new NotImplementedException();
        }

        /// Formats the key - keys are case sensitive!
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private string FormatKey(string key)
        {
            return key.ToLower();
        }

        /// <summary>
        /// Called when [written to].
        /// </summary>
        protected virtual void OnWrittenTo(string key, object entry)
        {
            if (WrittenTo != null)
            {
                WrittenTo(this, new EventArgs());
            }
        }

        /// <summary>
        /// Called when [read from].
        /// </summary>
        protected virtual void OnReadFrom(string key, object entry)
        {
            if (ReadFrom != null)
            {
                ReadFrom(this, new EventArgs());
            }
        }
    }
}