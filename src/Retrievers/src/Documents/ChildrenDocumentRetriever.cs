using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using CMS.DataEngine;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> A class that can query the children of Kentico Documents within the Content Tree. </summary>
    public class ChildrenDocumentRetriever : IChildrenDocumentRetriever
    {
        #region Fields
        private readonly IDocumentRetriever documentRetriever;
        #endregion

        public ChildrenDocumentRetriever( IDocumentRetriever documentRetriever )
            => this.documentRetriever = documentRetriever;

        /// <summary> Modifies a query by adding a where clause that filters nodes by their <see cref="TreeNode.NodeParentID"/>. </summary>
        /// <returns> The modified query. </returns>
        protected virtual TQuery ApplyChildFilter<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            return query.GetTypedQuery()
                  .WhereEquals( nameof( TreeNode.NodeParentID ), nodeID )
                  .ResetOrderBy( nameof( TreeNode.NodeOrder ), OrderDirection.Ascending );
        }

        private static Func<DocumentQuery<TNode>, DocumentQuery<TNode>> CreateChildrenOfChildQueryFilter<TNode>( Func<DocumentQuery<TNode>, DocumentQuery<TNode>> queryFilter = null )
            where TNode : TreeNode, new()
            => ( DocumentQuery<TNode> query )
                =>
                {
                    queryFilter?.Invoke( query );
                    return EnsureNodeIDSelected( query )
                        .TopN( 1 );
                };

        private static DocumentQuery<TNode> EnsureNodeIDSelected<TNode>( DocumentQuery<TNode> query )
            where TNode : TreeNode, new()
        {
            var typedQuery = query.GetTypedQuery();
            if(
                typedQuery.SelectColumnsList.AnyColumnsDefined
                    && !typedQuery.SelectColumnsList.Any( c => !c.IsSingleColumn || c.Name == nameof( TreeNode.NodeID ) )
            )
            {
                typedQuery.SelectColumnsList.Add( nameof( TreeNode.NodeID ) );
            }

            return typedQuery;
        }

        /// <inheritdoc />
        public virtual DocumentQuery<TNode> GetChildren<TNode>( int nodeID )
            where TNode : TreeNode, new()
            => ApplyChildFilter( documentRetriever.GetDocuments<TNode>(), nodeID );

        /// <inheritdoc />
        public virtual MultiDocumentQuery GetChildren( int nodeID )
            => ApplyChildFilter( documentRetriever.GetDocuments(), nodeID );

        /// <inheritdoc />
        public virtual (TChildNode, DocumentQuery<TNode>) GetChildrenOfChild<TChildNode, TNode>( int parentNodeID, Func<DocumentQuery<TChildNode>, DocumentQuery<TChildNode>> childQueryFilter = null )
            where TChildNode : TreeNode, new()
            where TNode : TreeNode, new()
        {
            var queryFilter = CreateChildrenOfChildQueryFilter( childQueryFilter );
            var node = queryFilter( GetChildren<TChildNode>( parentNodeID ) )
                ?.FirstOrDefault();

            return (
                node,
                node == null
                    ? new DocumentQuery<TNode>().NoResults()
                    : GetChildren<TNode>( node.NodeID )
            );
        }

        /// <inheritdoc />
        public virtual (TChildNode, MultiDocumentQuery) GetChildrenOfChild<TChildNode>( int parentNodeID, Func<DocumentQuery<TChildNode>, DocumentQuery<TChildNode>> childQueryFilter = null )
            where TChildNode : TreeNode, new()
        {
            var queryFilter = CreateChildrenOfChildQueryFilter( childQueryFilter );
            var node = queryFilter( GetChildren<TChildNode>( parentNodeID ) )
                ?.FirstOrDefault();

            return (
                node,
                node == null
                    ? new MultiDocumentQuery().NoResults()
                    : GetChildren( node.NodeID )
            );
        }

    }

}
