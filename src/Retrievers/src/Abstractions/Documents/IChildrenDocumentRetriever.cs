using System;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents
{

    /// <summary> Defines a class that can query the children of Documents within the Kentico Content Tree. </summary>
    public interface IChildrenDocumentRetriever
    {

        /// <summary> Query documents of the specified Page Type that are children of the <see cref="TreeNode"/> identified by the given <paramref name="nodeID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the parent <see cref="TreeNode"/>. </param>
        /// <returns> A query for the children of a node. </returns>
        DocumentQuery<TNode> GetChildren<TNode>( int nodeID )
            where TNode : TreeNode, new();

        /// <summary> Query documents of any Page Type that are children of the <see cref="TreeNode"/> identified by the given <paramref name="nodeID"/>. </summary>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the parent <see cref="TreeNode"/>. </param>
        /// <returns> A query for the children of a node. </returns>
        MultiDocumentQuery GetChildren( int nodeID );

        /// <summary> Query documents of the specified Page Type that are children of the child of the specified child Page Type, as filtered by the <paramref name="childQueryFilter"/>. </summary>
        /// <typeparam name="TChildNode"> The Page Type of the child whose children are queried. </typeparam>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="parentNodeID"> The <see cref="TreeNode.NodeID"/> of the parent <see cref="TreeNode"/>. </param>
        /// <param name="childQueryFilter"> A delegate method that is used to filter the query that selects the child whose children are retrieved. </param>
        /// <remarks> Implementations of this method may result in the execution of multiple queries. </remarks>
        /// <returns> A <see cref="Tuple"/> containing a child node of <paramref name="parentNodeID"/>, of type <typeparamref name="TChildNode"/>, and a query for the children of the child node, of type <typeparamref name="TNode"/>. </returns>
        (TChildNode, DocumentQuery<TNode>) GetChildrenOfChild<TChildNode, TNode>( int parentNodeID, Func<DocumentQuery<TChildNode>, DocumentQuery<TChildNode>> childQueryFilter = null )
            where TNode : TreeNode, new()
            where TChildNode : TreeNode, new();

        /// <summary> Query documents of any Page Type that are children of the child of the specified child Page Type, as filtered by the <paramref name="childQueryFilter"/>. </summary>
        /// <typeparam name="TChildNode"> The Page Type of the child whose children are queried. </typeparam>
        /// <param name="parentNodeID"> The <see cref="TreeNode.NodeID"/> of the parent <see cref="TreeNode"/>. </param>
        /// <param name="childQueryFilter"> A delegate method that is used to filter the query that selects the child whose children are retrieved. </param>
        /// <remarks> Implementations of this method may result in the execution of multiple queries. </remarks>
        /// <returns> A <see cref="Tuple"/> containing a child node of <paramref name="parentNodeID"/>, of type <typeparamref name="TChildNode"/>, and a query for the children of the child node, of any PageType. </returns>
        (TChildNode, MultiDocumentQuery) GetChildrenOfChild<TChildNode>( int parentNodeID, Func<DocumentQuery<TChildNode>, DocumentQuery<TChildNode>> childQueryFilter = null )
            where TChildNode : TreeNode, new();

    }

}
