using System;
using System.Linq;
using System.Threading;
using CMS.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    /// <summary> Extensions to <see cref="ICacheEntry"/>. </summary>
    public static class ICacheEntryExtensions
    {

        private static void OnDependencyEvicted( CancellationTokenSource tokenSource, object keyValue )
        {
            CacheHelper.RemoveDependencyCallback( keyValue.ToString() );

            tokenSource?.Cancel();
            tokenSource?.Dispose();
        }

        /// <summary> Registers the specified <paramref name="entry"/> with a dependency on the given Kentico dependency keys. </summary>
        /// <param name="entry"> The <see cref="ICacheEntry"/> to set a dependency for. </param>
        /// <param name="dependencies"> The Kentico dependency keys. </param>
        public static ICacheEntry SetCMSDependency( this ICacheEntry entry, params string[] dependencies )
        {
            if( dependencies?.Any() != true )
            {
                return entry;
            }

            return SetCMSDependency(
               entry,
               CacheHelper.GetCacheDependency( dependencies )
           );
        }

        /// <summary> Registers the specified <paramref name="entry"/> with a dependency on the given Kentico dependency. </summary>
        /// <param name="entry"> The <see cref="ICacheEntry"/> to set a dependency for. </param>
        /// <param name="dependency"> The Kentico dependency to expire the <paramref name="entry"/> on. </param>
        public static ICacheEntry SetCMSDependency( this ICacheEntry entry, CMSCacheDependency dependency )
        {
            if( dependency == null )
            {
                throw new ArgumentNullException( nameof( dependency ) );
            }

#pragma warning disable CA2000 // Dispose objects before losing scope

            // no using; reference is passed to and disposed of within the callback
            var tokenSource = new CancellationTokenSource();
#pragma warning restore CA2000

            dependency.EnsureDummyKeys();

            string key = $"cachecallback|{Guid.NewGuid()}";
            CacheHelper.RegisterDependencyCallback( key, dependency, tokenSource, OnDependencyEvicted, key );

            entry.AddExpirationToken( new CancellationChangeToken( tokenSource.Token ) );
            return entry;
        }

        /// <summary> Registers the specified <paramref name="entry"/> with a dependency on the configured Kentico dependency. </summary>
        /// <param name="entry"> The entry to set a dependency for. </param>
        /// <param name="configureDependency"> A method that configures the Kentico dependency. </param>
        public static ICacheEntry WithCMSDependency( this ICacheEntry entry, Action<CMSCacheDependency> configureDependency )
        {
            if( configureDependency == null )
            {
                throw new ArgumentNullException( nameof( configureDependency ) );
            }

            var dependency = new CMSCacheDependency( null, null, DateTime.Now );
            configureDependency( dependency );

            return SetCMSDependency( entry, dependency );
        }

    }

}
