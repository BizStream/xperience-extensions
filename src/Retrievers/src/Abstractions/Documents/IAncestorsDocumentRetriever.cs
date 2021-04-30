using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents
{

    /// <summary> Defines a class that can query the ancestors of Documents within the Kentico Content Tree. </summary>
    public interface IAncestorsDocumentRetriever
    {

        /// <summary> Query documents of the specified Page Type that are ancestors of the <see cref="TreeNode"/> identified by <paramref name="nodeID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the descendant node. </param>
        DocumentQuery<TNode> GetAncestors<TNode>( int nodeID )
            where TNode : TreeNode, new();

        /// <summary> Query documents of any Page Type that are ancestors of the <see cref="TreeNode"/> identified by <paramref name="nodeID"/>. </summary>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the descendant node. </param>
        MultiDocumentQuery GetAncestors( int nodeID );

        /// <summary> Query the first document of the specified Page Type that is an ancestor of the <see cref="TreeNode"/> identified by <paramref name="nodeID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the descendant node. </param>
        TNode GetAncestor<TNode>( int nodeID )
            where TNode : TreeNode, new();

    }

}
