using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.DocumentEngine
{

    /// <summary> Extensions to <see cref="IDocumentQuery"/> and <see cref="IDocumentQuery{TQuery, TObject}"/>. </summary>
    public static class IDocumentQueryExtensions
    {

        /// <summary> Get the name of the Sql alias used for the query based on the type of query. </summary>
        /// <see cref="Strings.DocumentQueryAliasName"/>
        /// <see cref="Strings.MultiDocumentQueryAliasName"/>
        public static string GetQueryAliasName( this IDocumentQuery query )
            => query is MultiDocumentQuery
                ? Strings.MultiDocumentQueryAliasName
                : Strings.DocumentQueryAliasName;

    }

}
