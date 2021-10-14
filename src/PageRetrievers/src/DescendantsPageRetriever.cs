using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using BizStream.Extensions.Kentico.Xperience.DocumentEngine;
using BizStream.Extensions.Kentico.Xperience.PageRetrievers.Abstractions;
using CMS.DataEngine;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

namespace BizStream.Extensions.Kentico.Xperience.PageRetrievers
{

    public class DescendantsPageRetriever : IDescendantsPageRetriever
    {
        #region Fields
        private const string CteAliasName = "_descendants";
        private const string CteName = "Descendants";
        private const string DescendantNodeIDColumnName = "_DescendantNodeID";

        private readonly IPageRetriever pageRetriever;
        #endregion

        public DescendantsPageRetriever( IPageRetriever pageRetriever )
            => this.pageRetriever = pageRetriever;

        protected virtual void ApplyDescendantsQueryParameters<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID, Action<TQuery>? queryFilter )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            var treeAliasName = "Tree";
            var cteQuery = new DataQuery().From( SystemViewNames.View_CMS_Tree_Joined )
                .Column( nameof( TreeNode.NodeID ) )
                .WhereEquals( nameof( TreeNode.NodeParentID ), nodeID )
                .UnionAll(
                    new DataQuery().From( new QuerySourceTable( SystemViewNames.View_CMS_Tree_Joined, treeAliasName ) )
                        .Column( $"{treeAliasName}.{nameof( TreeNode.NodeID )}" )
                        .Source(
                            source => source.InnerJoin(
                                new QuerySourceTable( CteName, CteAliasName ),
                                $"{treeAliasName}.{nameof( TreeNode.NodeParentID )}",
                                $"{CteAliasName}.{nameof( TreeNode.NodeID )}"
                            )
                        )
                );

            string aliasName = query.GetQueryAliasName();
            query.WithCte( CteName, cteQuery )
                .Source(
                    source => source.Join(
                        new DataQuery().From( CteName )
                            .AsSingleColumn( $"{nameof( TreeNode.NodeID )} AS {DescendantNodeIDColumnName}", true ),
                        CteAliasName,
                        $"{aliasName}.{nameof( TreeNode.NodeID )}",
                        $"{CteAliasName}.{DescendantNodeIDColumnName}"
                    )
                )
                .ResetOrderBy( nameof( TreeNode.NodeLevel ), OrderDirection.Ascending );

            queryFilter?.Invoke( query.GetTypedQuery() );
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TNode>> RetrieveAsync<TNode>( int nodeID, Action<DocumentQuery<TNode>>? queryFilter = null, Action<IPageCacheBuilder<TNode>>? configureCache = null, CancellationToken cancellation = default )
            where TNode : TreeNode, new()
            => await pageRetriever.RetrieveAsync( query => ApplyDescendantsQueryParameters( query, nodeID, queryFilter ), configureCache, cancellation );

        /// <inheritdoc/>
        public async Task<IEnumerable<TreeNode>> RetrieveMultipleAsync( int nodeID, Action<MultiDocumentQuery>? queryFilter = null, Action<IPageCacheBuilder<TreeNode>>? configureCache = null, CancellationToken cancellation = default )
            => await pageRetriever.RetrieveMultipleAsync( query => ApplyDescendantsQueryParameters( query, nodeID, queryFilter ), configureCache, cancellation );

    }

}
