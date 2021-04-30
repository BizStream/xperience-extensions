using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents
{

    /// <summary> Defines a class that can query the descendants of Documents in the Kentico Content Tree. </summary>
    public interface IDescendantsDocumentRetriever
    {

        /// <summary> Query documents of the specified Page Type that are descendants of the <see cref="TreeNode"/> identified by <paramref name="nodeID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the ancestor node. </param>
        DocumentQuery<TNode> GetDescendants<TNode>( int nodeID )
            where TNode : TreeNode, new();

        /// <summary> Query documents of any Page Type that are descendants of the <see cref="TreeNode"/> identified by <paramref name="nodeID"/>. </summary>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the ancestor node. </param>
        MultiDocumentQuery GetDescendants( int nodeID );

    }

}
