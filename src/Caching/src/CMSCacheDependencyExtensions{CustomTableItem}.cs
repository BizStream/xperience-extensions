using System;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using CMS.CustomTables;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    public static partial class CMSCacheDependencyExtensions
    {

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="CustomTableItem"/>, of type <typeparamref name="TItem"/>, identified by the given <paramref name="itemID"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="itemID"> The <see cref="CustomTableItem.ItemID"/> that identifies the <see cref="CustomTableItem"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnCustomTableItem<TItem>( this CMSCacheDependency dependency, int itemID )
            where TItem : CustomTableItem
        {
            if( dependency is null )
            {
                throw new ArgumentNullException( nameof( dependency ) );
            }

            var itemType = typeof( TItem );
            var className = itemType.GetCustomTableClassNameValue();
            if( string.IsNullOrEmpty( className ) )
            {
                throw new ArgumentException( $"Invalid {nameof( CustomTableItem )} given: {itemType.Name}", nameof( TItem ) );
            }

            return dependency.EnsureCacheKeys( $"customtableitem.{className}|byid|{itemID}" );
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on any (all) <see cref="CustomTableItem"/>s of type <typeparamref name="TItem"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnCustomTableItems<TItem>( this CMSCacheDependency dependency )
            where TItem : CustomTableItem
        {
            if( dependency is null )
            {
                throw new ArgumentNullException( nameof( dependency ) );
            }

            var itemType = typeof( TItem );
            var className = itemType.GetCustomTableClassNameValue();
            if( string.IsNullOrEmpty( className ) )
            {
                throw new ArgumentException( $"Invalid {nameof( CustomTableItem )} given: {itemType.Name}", nameof( TItem ) );
            }

            return dependency.EnsureCacheKeys( $"customtableitem.{className}|all" );
        }

    }

}
