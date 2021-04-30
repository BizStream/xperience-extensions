using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents
{

    /// <summary> Defines a class than can query for the siblings of Kentico Documents. </summary>
    public interface ISiblingsDocumentRetriever
    {

        /// <summary> Query documents of the specified Page Type that are siblings of the <see cref="TreeNode"/> identified by the given <paramref name="nodeID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the sibling <see cref="TreeNode"/>. </param>
        DocumentQuery<TNode> GetSiblings<TNode>( int nodeID )
            where TNode : TreeNode, new();

        /// <summary> Query documents of the specified Page Type that are siblings of the <see cref="TreeNode"/> identified by the given <paramref name="nodeID"/>. </summary>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the sibling <see cref="TreeNode"/>. </param>
        MultiDocumentQuery GetSiblings( int nodeID );

    }

}
