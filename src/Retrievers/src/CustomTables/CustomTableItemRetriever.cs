using BizStream.Extensions.Kentico.Xperience.DataEngine;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.CustomTables;
using CMS.CustomTables;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.CustomTables
{

    /// <summary> A class that can query Kentico Custom Table Items. </summary>
    public class CustomTableItemRetriever : ICustomTableItemRetriever
    {

        /// <inheritdoc />
        public virtual ObjectQuery<TCustomTableItem> GetItems<TCustomTableItem>( )
            where TCustomTableItem : CustomTableItem, new()
            => CustomTableItemProvider.GetItems<TCustomTableItem>()
                .ResetOrderBy( nameof( CustomTableItem.ItemOrder ) );

    }
}
