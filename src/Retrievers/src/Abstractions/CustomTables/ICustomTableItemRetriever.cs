using CMS.CustomTables;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.CustomTables
{

    /// <summary> Defines a class that can query items of a Kentico Custom Table. </summary>
    public interface ICustomTableItemRetriever
    {

        /// <summary> Query Custom Table Items of the specified type. </summary>
        /// <typeparam name="TCustomTableItem"> The type of Custom Table Item to query. </typeparam>
        ObjectQuery<TCustomTableItem> GetItems<TCustomTableItem>( )
            where TCustomTableItem : CustomTableItem, new();

    }

}
