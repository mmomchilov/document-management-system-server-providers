using System;
using System.Threading.Tasks;
using Pirina.Providers.MemoryCache.L0.Tests.MockData;
using MemoryCacheProvider;
using NUnit.Framework;

namespace Pirina.Providers.MemoryCache.L0.Tests
{
    [TestFixture]
    public class MemoryCacheTests
    {
        [Test]
        public void PutEntry_default_policy()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT
            cache.Put(key, entry);
            var cached = cache.Get(key);
            //ASSERT
            Assert.AreSame(entry, cached);
        }

        [Test]
        public void PutEntry_default_policy_write_event_fired()
        {
            //ARRANGE
            var writtenTo = false;
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            cache.WrittenTo += (o, e) => { writtenTo = true; };
            //ACT
            cache.Put(key, entry);
            var cached = cache.Get(key);
            //ASSERT
            Assert.AreSame(entry, cached);
            Assert.IsTrue(writtenTo);
        }

        [Test]
        public void PutEntry_default_policy_replace_the_entry_by_calling_put_again()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry1 = new MockEntryToCache { PropertyInt = 1 };
            var entry2 = new MockEntryToCache { PropertyInt = 2 };
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT
            cache.Put(key, entry1);
            var cachedEntry1 = cache.Get(key);
            cache.Put(key, entry2);
            var cachedEntry2 = cache.Get(key);
            //ASSERT
            Assert.AreSame(entry1, cachedEntry1);
            Assert.AreSame(entry2, cachedEntry2);
        }

        [Test]
        public void GetEntry_which_not_exist()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT
            
            //ASSERT
            Assert.Throws<InvalidOperationException>(() => cache.Get(key));
        }

        [Test]
        public void TryGetEntry_which_not_exist()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            object item;
            //ACT
            var found = cache.TryGet(key, out item);
            //ASSERT
            Assert.IsFalse(found);
            Assert.IsNull(item);
        }

        [Test]
        public void Try_get_Entry_which_exist()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            object item;
            //ACT
            cache.Put(key, entry);

            var found = cache.TryGet(key, out item);
            //ASSERT
            Assert.AreSame(entry, item);
            Assert.IsTrue(found);
        }

        [Test]
        public void GetEntry_which_not_exist_read_event_fired()
        {
            //ARRANGE
            var read = false;
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            cache.ReadFrom += (o, e) => { read = true; };
            //ACT

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => cache.Get(key));
            Assert.IsTrue(read);
        }

        [Test]
        public void GetEntry_generic_which_not_exist()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT

            //ASSERT
            Assert.Throws<InvalidOperationException>(() => cache.Get<MockEntryToCache>(key));
        }

        [Test]
        public void TryGetEntry_generic_which_not_exist()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            MockEntryToCache item;
            //ACT
            var found = cache.TryGet<MockEntryToCache>(key, out item);
            //ASSERT
            Assert.IsFalse(found);
            Assert.IsNull(item);
        }

        [Test]
        public void Try_get_Entry_generic_which_exist()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            MockEntryToCache item;
            //ACT
            cache.Put(key, entry);

            var found = cache.TryGet<MockEntryToCache>(key, out item);
            //ASSERT
            Assert.AreSame(entry, item);
            Assert.IsTrue(found);
        }

        [Test]
        public void GetEntry_generic_invalid_cast()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT
            cache.Put(key, entry);

            //ASSERT
            Assert.Throws<InvalidCastException>(() => cache.Get<string>(key));
        }
        [Test]
        public async Task GetOrCreate_Entry_default_policy_create_entry()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT
            
            var cached = await cache.GetOrAddAsync(key, _ => Task.FromResult(entry));
            //ASSERT
            Assert.AreSame(entry, cached);
        }

        [Test]
        public async Task GetOrCreate_Entry_default_policy_get_entry()
        {
            //ARRANGE
            var key = string.Format("_key_{0}", Guid.NewGuid());
            var entry = new MockEntryToCache();
            var entry1 = new MockEntryToCache();
            var cache = new MemoryCacheRuntimeImplementor();
            //ACT
            var cached = await cache.GetOrAddAsync(key, _ => Task.FromResult(entry));
            var cached1 = await cache.GetOrAddAsync(key, _ => Task.FromResult(entry1));
            //ASSERT
            Assert.AreSame(entry, cached);
            Assert.AreSame(entry, cached1);
        }
    }
}