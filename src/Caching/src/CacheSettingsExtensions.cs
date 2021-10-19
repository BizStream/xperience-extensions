using System;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{
    public static class CacheSettingsExtensions
    {
        public static CacheSettings WithCacheDependency( this CacheSettings settings, CMSCacheDependency dependency )
        {
            if( settings is null )
            {
                throw new ArgumentNullException( nameof( settings ) );
            }

            settings.GetCacheDependency = ( ) => dependency;
            return settings;
        }

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
