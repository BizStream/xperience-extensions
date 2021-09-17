using System;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using CMS.DataEngine;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.DocumentEngine
{

    /// <summary> Extensions to <see cref="IDocumentQuery"/> and <see cref="IDocumentQuery{TQuery, TObject}"/>. </summary>
    public static class IDocumentQueryExtensions
    {

        /// <summary> Filters the query to pages of the given PageType that exist at the root of the Content Tree. </summary>
        /// <param name="query"> The query to filter. </param>
        /// <typeparam name="TNode"> The PageType to filter by. </typeparam>
        public static TQuery AtRootLevel<TQuery, TNode>( this IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            var typedQuery = query.GetTypedQuery();
            return typedQuery.NestingLevel( 1 )
                .ResetOrderBy( nameof( TreeNode.NodeOrder ), OrderDirection.Descending );
        }

        /// <summary> Get the name of the Sql alias used for the query based on the type of query. </summary>
        /// <see cref="DocumentQueryStrings.DocumentQueryAliasName"/>
        /// <see cref="DocumentQueryStrings.MultiDocumentQueryAliasName"/>
        public static string GetQueryAliasName( this IDocumentQuery query )
            => query is MultiDocumentQuery
                ? DocumentQueryStrings.MultiDocumentQueryAliasName
                : DocumentQueryStrings.DocumentQueryAliasName;

    }

}
