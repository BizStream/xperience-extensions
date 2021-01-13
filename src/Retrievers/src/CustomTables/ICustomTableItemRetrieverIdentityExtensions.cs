using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.CustomTables;
using CMS.CustomTables;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.CustomTables
{

    /// <summary> Extensions to <see cref="ICustomTableItemRetriever"/> that support querying a single item common unique identifies. </summary>
    public static class ICustomTableItemRetrieverIdentityExtensions
    {

        /// <summary> Query a Custom Table Item by its ID. </summary>
        /// <typeparam name="TCustomTableItem"> The type of Custom Table Item to query. </typeparam>
        /// <param name="itemID"> The <see cref="CustomTableItem.ItemID"/> of the item to query. </param>
        /// <returns> The Custom Table Item with the given <paramref name="itemID"/>. </returns>
        public static TCustomTableItem GetItem<TCustomTableItem>( this ICustomTableItemRetriever itemRetriever, int itemID )
            where TCustomTableItem : CustomTableItem, new()
        {
            ThrowIfRetrieverIsNull( itemRetriever );
            return itemRetriever.GetItems<TCustomTableItem>()
                .WhereEquals( nameof( CustomTableItem.ItemID ), itemID )
                .TopN( 1 )
                .FirstOrDefault();
        }

        /// <summary> Query a Custom Table Item by its Guid. </summary>
        /// <typeparam name="TCustomTableItem"> The type of Custom Table Item to query. </typeparam>
        /// <param name="itemGuid"> The <see cref="CustomTableItem.ItemGUID"/> of the item to query. </param>
        /// <returns> The Custom Table Item with the given <paramref name="itemGuid"/>. </returns>
        public static TCustomTableItem GetItem<TCustomTableItem>( this ICustomTableItemRetriever itemRetriever, Guid itemGuid )
            where TCustomTableItem : CustomTableItem, new()
        {
            ThrowIfRetrieverIsNull( itemRetriever );
            return itemRetriever.GetItems<TCustomTableItem>()
                .WhereEquals( nameof( CustomTableItem.ItemGUID ), itemGuid )
                .TopN( 1 )
                .FirstOrDefault();
        }

        private static void ThrowIfRetrieverIsNull( ICustomTableItemRetriever itemRetriever )
        {
            if( itemRetriever == null )
            {
                throw new ArgumentNullException( nameof( itemRetriever ) );
            }
        }

    }

}
