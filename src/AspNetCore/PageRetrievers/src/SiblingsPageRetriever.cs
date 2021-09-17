using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers.Abstractions;
using CMS.DataEngine;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

namespace BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers
{

    public class SiblingsPageRetriever : IAncestorsPageRetriever
    {
        #region Fields
        private readonly IPageRetriever pageRetriever;
        #endregion

        public SiblingsPageRetriever( IPageRetriever pageRetriever )
            => this.pageRetriever = pageRetriever;

        protected virtual void ApplySiblingsQueryParameters<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID, Action<TQuery>? filterQuery )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            query.WhereEquals(
                nameof( TreeNode.NodeParentID ),
                new DataQuery().From( SystemViewNames.View_CMS_Tree_Joined )
                    .WhereEquals( nameof( TreeNode.NodeID ), nodeID )
                    .TopN( 1 )
                    .AsSingleColumn( nameof( TreeNode.NodeParentID ), true )
            );

            filterQuery?.Invoke( query.GetTypedQuery() );
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TNode>> RetrieveAsync<TNode>( int nodeID, Action<DocumentQuery<TNode>>? filterQuery = null, Action<IPageCacheBuilder<TNode>>? configureCache = null, CancellationToken cancellation = default )
            where TNode : TreeNode, new()
            => await pageRetriever.RetrieveAsync( query => ApplySiblingsQueryParameters( query, nodeID, filterQuery ), configureCache, cancellation );

        /// <inheritdoc/>
        public async Task<IEnumerable<TreeNode>> RetrieveMultipleAsync( int nodeID, Action<MultiDocumentQuery>? filterQuery = null, Action<IPageCacheBuilder<TreeNode>>? configureCache = null, CancellationToken cancellation = default )
            => await pageRetriever.RetrieveMultipleAsync( query => ApplySiblingsQueryParameters( query, nodeID, filterQuery ), configureCache, cancellation );
    }

}
