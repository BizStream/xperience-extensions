using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using BizStream.Extensions.Kentico.Xperience.PageRetrievers.Abstractions;
using CMS.DataEngine;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

namespace BizStream.Extensions.Kentico.Xperience.PageRetrievers
{

    public class ChildrenPageRetriever : IChildrenPageRetriever
    {
        #region Fields
        private readonly IPageRetriever pageRetriever;
        #endregion

        public ChildrenPageRetriever( IPageRetriever pageRetriever )
            => this.pageRetriever = pageRetriever;

        protected virtual void ApplyChildrenQueryParameters<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID, Action<TQuery>? queryFilter )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            query.WhereEquals( nameof( TreeNode.NodeParentID ), nodeID )
                .ResetOrderBy( nameof( TreeNode.NodeOrder ), OrderDirection.Ascending );

            queryFilter?.Invoke( query.GetTypedQuery() );
        }

        public async Task<IEnumerable<TNode>> RetrieveAsync<TNode>( int nodeID, Action<DocumentQuery<TNode>>? queryFilter = null, Action<IPageCacheBuilder<TNode>>? configureCache = null, CancellationToken cancellation = default )
            where TNode : TreeNode, new()
            => await pageRetriever.RetrieveAsync( query => ApplyChildrenQueryParameters( query, nodeID, queryFilter ), configureCache, cancellation );

        public async Task<IEnumerable<TreeNode>> RetrieveMultipleAsync( int nodeID, Action<MultiDocumentQuery>? queryFilter = null, Action<IPageCacheBuilder<TreeNode>>? configureCache = null, CancellationToken cancellation = default )
            => await pageRetriever.RetrieveMultipleAsync( query => ApplyChildrenQueryParameters( query, nodeID, queryFilter ), configureCache, cancellation );

    }

}
