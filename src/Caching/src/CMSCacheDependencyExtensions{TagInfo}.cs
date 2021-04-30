using System;
using CMS.Helpers;
using CMS.Taxonomy;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    public static partial class CMSCacheDependencyExtensions
    {

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TagInfo"/> identified by the given <paramref name="tagName"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="tagName"> The <see cref="TagInfo.TagName"/> that identifies the <see cref="TagInfo"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnTag( this CMSCacheDependency dependency, string tagName )
            => OnObject<TagInfo>( dependency, tagName );

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TagInfo"/> identified by the given <paramref name="tagName"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="tagID"> The <see cref="TagInfo.TagID"/> that identifies the <see cref="TagInfo"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnTag( this CMSCacheDependency dependency, int tagID )
            => OnObject<TagInfo>( dependency, tagID );

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TagInfo"/> identified by the given <paramref name="tagName"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="tagGuid"> The <see cref="TagInfo.TagGUID"/> that identifies the <see cref="TagInfo"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnTag( this CMSCacheDependency dependency, Guid tagGuid )
            => OnObject<TagInfo>( dependency, tagGuid );

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on the <see cref="TagInfo"/>s identified by the given <paramref name="tagNames"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="tagNames"> The <see cref="TagInfo.TagName"/>s that identify the <see cref="TagInfo"/>s to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnTags( this CMSCacheDependency dependency, params string[] tagNames )
        {
            ThrowIfDependencyIsNull( dependency );
            foreach( var tagName in tagNames )
            {
                OnObject<TagInfo>( dependency, tagName );
            }

            return dependency;
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on any (all) <see cref="TagInfo"/>s. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnTags( this CMSCacheDependency dependency )
            => OnObjectsOfType<TagInfo>( dependency );

    }

}
