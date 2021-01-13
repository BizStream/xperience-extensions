using System;
using System.Collections.Generic;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using CMS.DataEngine;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    /// <summary> Extensions to <see cref="CMSCacheDependency"/> to assist with fluently configuring dependency keys. </summary>
    public static partial class CMSCacheDependencyExtensions
    {

        private static CMSCacheDependency EnsureCacheKeys( this CMSCacheDependency dependency, params string[] keysToAdd )
        {
            ThrowIfDependencyIsNull( dependency );

            var keys = new HashSet<string>(
                dependency?.CacheKeys ?? Enumerable.Empty<string>(),
                StringComparer.InvariantCultureIgnoreCase
            );

            foreach( var keyToAdd in keysToAdd )
            {
                keys.Add( keyToAdd );
            }

            dependency.CacheKeys = keys.ToArray();
            return dependency;
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on an object of type <typeparamref name="TInfo"/>, identified by the given <paramref name="codeName"/>. </summary>
        /// <typeparam name="TInfo"> The type of Kentico Object to configure the dependency with. </typeparam>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="codeName"> The value of the <see cref="BaseInfo.CodeNameColumn"/> for an instance of a Kentico object of type <typeparamref name="TInfo"/>. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        /// <remarks> This method expects the type <typeparamref name="TInfo"/> to define a <c>OBJECT_TYPE</c> constant (resolved via reflection). </remarks>
        public static CMSCacheDependency OnObject<TInfo>( this CMSCacheDependency dependency, string codeName )
            where TInfo : AbstractInfo<TInfo>, new()
        {
            ThrowIfDependencyIsNull( dependency );

            var objectType = typeof( TInfo ).GetObjectTypeValue();
            ThrowIfObjectTypeIsNull( objectType );

            return EnsureCacheKeys( dependency, $"{objectType}|byname|{codeName}" );
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on any (all) objects of type <typeparamref name="TInfo"/>. </summary>
        /// <typeparam name="TInfo"> The type of Kentico Object to configure the dependency with. </typeparam>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        /// <remarks> This method expects the type <typeparamref name="TInfo"/> to define a <c>OBJECT_TYPE</c> constant (resolved via reflection). </remarks>
        public static CMSCacheDependency OnObjectsOfType<TInfo>( this CMSCacheDependency dependency )
            where TInfo : AbstractInfo<TInfo>, new()
        {
            ThrowIfDependencyIsNull( dependency );

            var objectType = typeof( TInfo ).GetObjectTypeValue();
            ThrowIfObjectTypeIsNull( objectType );

            return EnsureCacheKeys( dependency, $"{objectType}|all" );
        }

        private static void ThrowIfDependencyIsNull( CMSCacheDependency dependency )
        {
            if( dependency == null )
            {
                throw new ArgumentNullException( nameof( dependency ) );
            }
        }

        private static void ThrowIfObjectTypeIsNull( string objectType )
        {
            if( string.IsNullOrWhiteSpace( objectType ) )
            {
                throw new InvalidOperationException( "Unable to determine objectType." );
            }
        }

        private static void ThrowIsSiteNameIsEmpty( string siteName )
        {
            if( string.IsNullOrWhiteSpace( siteName ) )
            {
                throw new ArgumentNullException( nameof( siteName ) );
            }
        }

    }

}
