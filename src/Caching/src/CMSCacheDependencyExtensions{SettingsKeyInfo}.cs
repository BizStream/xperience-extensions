using CMS.DataEngine;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    public static partial class CMSCacheDependencyExtensions
    {

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="SettingsKeyInfo"/> identified by the given <paramref name="key"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="key"> The <see cref="SettingsKeyInfo.KeyName"/> that identifies the <see cref="SettingsKeyInfo"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnSettingsKey( this CMSCacheDependency dependency, string key )
        {
            ThrowIfDependencyIsNull( dependency );

            OnObject<SettingsKeyInfo>( dependency, key );
            return dependency;
        }

    }

}
