using System;
using CMS.Ecommerce;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    public static partial class CMSCacheDependencyExtensions
    {

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="SKUInfo"/> identified by the given <paramref name="tagName"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="skuID"> The <see cref="SKUInfo.SKUID"/> that identifies the <see cref="SKUInfo"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnSku( this CMSCacheDependency dependency, int skuID )
        {
            ThrowIfDependencyIsNull( dependency );

            OnObject<SKUInfo>( dependency, skuID );
            return dependency;
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="SKUInfo"/> identified by the given <paramref name="tagName"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="skuGuid"> The <see cref="SKUInfo.SKUGUID"/> that identifies the <see cref="SKUInfo"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnSku( this CMSCacheDependency dependency, Guid skuGuid )
        {
            ThrowIfDependencyIsNull( dependency );

            OnSku( dependency, skuGuid );
            return dependency;
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on the given <see cref="SKUInfo"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="sku"> The <see cref="SKUInfo"/> to configure the dependency on. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        /// <seealso cref="OnSku(CMSCacheDependency, int)"/>
        /// <seealso cref="OnSku(CMSCacheDependency, Guid)"/>
        public static CMSCacheDependency OnSku( this CMSCacheDependency dependency, SKUInfo sku )
        {
            ThrowIfDependencyIsNull( dependency );

            OnSku( dependency, sku.SKUID );
            OnSku( dependency, sku.SKUGUID );
            return dependency;
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on any (all) <see cref="SKUInfo"/>s. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnSkus( this CMSCacheDependency dependency )
        {
            ThrowIfDependencyIsNull( dependency );

            EnsureCacheKeys( dependency, $"{SKUInfo.OBJECT_TYPE_SKU}|all" );
            return dependency;
        }

    }

}
