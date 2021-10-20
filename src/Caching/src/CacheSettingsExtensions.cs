using System;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{
    /// <summary> Extensions to <see cref="CacheSettings"/> to assist with Fluent configuration of settings. </summary>
    public static class CacheSettingsExtensions
    {
        /// <summary> Configure the <see cref="CacheSettings"/> dependencies. </summary>
        /// <param name="settings"> The <see cref="CacheSettings"/> to configure. </param>
        /// <param name="dependency"> The configured dependency object. </param>
        public static CacheSettings WithCacheDependency( this CacheSettings settings, CMSCacheDependency dependency )
        {
            if( settings is null )
            {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings.GetCacheDependency = ( ) => dependency;
            return settings;
        }

        /// <summary> Configure the <see cref="CacheSettings"/> dependencies. </summary>
        /// <param name="settings"> The <see cref="CacheSettings"/> to configure. </param>
        /// <param name="configure"> A delegate that configures a <see cref="CMSCacheDependency"/> for the <paramref name="settings"/>. </param>
        public static CacheSettings WithCacheDependency( this CacheSettings settings, Action<CMSCacheDependency> configure )
        {
            if( settings is null )
            {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings.GetCacheDependency = ( ) =>
            {
                CMSCacheDependency dependency = new( null, null, DateTime.Now );

                configure( dependency );
                return dependency;
            };

            return settings;
        }
    }
}
