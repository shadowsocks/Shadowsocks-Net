/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */


using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Buffers;
using Argument.Check;

namespace Shadowsocks.Infrastructure
{

    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycacheentryoptions?view=dotnet-plat-ext-3.1
    //https://github.com/aspnet/Caching/blob/master/samples/MemoryCacheConcurencySample/Program.cs

    public sealed class LruCache<TValue>
    {
        IMemoryCache _cache = null;

        public LruCache()
            : this(TimeSpan.FromSeconds(30))
        {

        }
        public LruCache(TimeSpan expirationScanFrequency)
        {
            var option = new MemoryCacheOptions();
            option.CompactionPercentage = .7D;

            option.ExpirationScanFrequency = expirationScanFrequency;
            _cache = new MemoryCache(option);

        }

        public TValue Get(object key)
        {
            return _cache.Get<TValue>(key);
        }


        //TODO need a better LRU cache, //there is no scan timer at all.
        public TValue Set(object key, TValue value, TimeSpan slidingExpiration, PostEvictionCallbackRegistration evictionCallback = null)
        {
            var opt = new MemoryCacheEntryOptions();
            ////opt.AbsoluteExpirationRelativeToNow = slidingExpiration;           
            opt.SlidingExpiration = slidingExpiration;
            if (null != evictionCallback)
            {
                opt.PostEvictionCallbacks.Add(evictionCallback);
            }
            return _cache.Set(key, value, opt);
        }

        public TValue SetAbsoluteExpire(object key, TValue value, TimeSpan absoluteExpirationRelativeToNow, PostEvictionCallbackRegistration evictionCallback = null)
        {
            var opt = new MemoryCacheEntryOptions();
            opt.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            if (null != evictionCallback)
            {
                opt.PostEvictionCallbacks.Add(evictionCallback);
            }
            return _cache.Set(key, value, opt);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }
    }
}
