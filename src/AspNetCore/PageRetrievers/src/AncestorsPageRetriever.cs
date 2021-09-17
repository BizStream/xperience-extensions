using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers.Abstractions;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using BizStream.Extensions.Kentico.Xperience.DocumentEngine;
using CMS.DataEngine;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

namespace BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers
{

    public class AncestorsPageRetriever : IAncestorsPageRetriever
    {
        #region Fields
        private const string CteAliasName = "_ancestor";
        private const string CteName = "Ancestors";
        private const string AncestorNodeIDColumnName = "_AncestorNodeID";
        private readonly string[] cteColumns = new[] { nameof( TreeNode.NodeID ), nameof( TreeNode.NodeParentID ) };

        private readonly IPageRetriever pageRetriever;
        #endregion

        public AncestorsPageRetriever( IPageRetriever pageRetriever )
            => this.pageRetriever = pageRetriever;

        protected virtual void ApplyAncestorsQueryParameters<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID, Action<TQuery>? queryFilter )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            var treeAliasName = "Tree";
            var cteQuery = new DataQuery().From( SystemViewNames.View_CMS_Tree_Joined )
                .Columns( cteColumns )
                .WhereEquals(
                    nameof( TreeNode.NodeID ),
                    new DataQuery().From( SystemViewNames.View_CMS_Tree_Joined )
                        .Column( nameof( TreeNode.NodeParentID ) )
                        .WhereEquals( nameof( TreeNode.NodeID ), nodeID )
                        .TopN( 1 )
                        .AsSingleColumn( nameof( TreeNode.NodeParentID ), true )
                )
                .TopN( 1 )
                .UnionAll(
                    new DataQuery().From( new QuerySourceTable( SystemViewNames.View_CMS_Tree_Joined, treeAliasName ) )
                        .Columns( cteColumns.Select( column => $"{treeAliasName}.{column}" ) )
                        .Source(
                            source => source.InnerJoin(
                                new QuerySourceTable( CteName, CteAliasName ),
                                $"{treeAliasName}.{nameof( TreeNode.NodeID )}",
                                $"{CteAliasName}.{nameof( TreeNode.NodeParentID )}"
                            )
                        )
                );

            string aliasName = query.GetQueryAliasName();
            query.WithCte( CteName, cteQuery, cteColumns )
                .Source(
                    source => source.Join(
                        new DataQuery().From( CteName )
                            .AsSingleColumn( $"{nameof( TreeNode.NodeID )} as {AncestorNodeIDColumnName}" ),
                        CteAliasName,
                        $"{aliasName}.{nameof( TreeNode.NodeID )}",
                        $"{CteAliasName}.{AncestorNodeIDColumnName}",
                        new WhereCondition( $"{aliasName}.{nameof( TreeNode.NodeLevel )}", QueryOperator.GreaterOrEquals, 1 ),
                        JoinTypeEnum.Inner
                    )
                )
                .ResetOrderBy( nameof( TreeNode.NodeLevel ), OrderDirection.Descending );

            queryFilter?.Invoke( query.GetTypedQuery() );
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TNode>> RetrieveAsync<TNode>( int nodeID, Action<DocumentQuery<TNode>>? queryFilter = null, Action<IPageCacheBuilder<TNode>>? configureCache = null, CancellationToken cancellation = default )
            where TNode : TreeNode, new()
            => await pageRetriever.RetrieveAsync( query => ApplyAncestorsQueryParameters( query, nodeID, queryFilter ), configureCache, cancellation );

        /// <inheritdoc/>
        public async Task<IEnumerable<TreeNode>> RetrieveMultipleAsync( int nodeID, Action<MultiDocumentQuery>? queryFilter = null, Action<IPageCacheBuilder<TreeNode>>? configureCache = null, CancellationToken cancellation = default )
            => await pageRetriever.RetrieveMultipleAsync( query => ApplyAncestorsQueryParameters( query, nodeID, queryFilter ), configureCache, cancellation );
    }

}
