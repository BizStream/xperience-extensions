using System;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> Extensions to <see cref="IChildrenDocumentRetriever"/>. </summary>
    public static class IChildrenDocumentRetrieverExtensions
    {

        /// <summary> Query documents of the specified Page Type that are children of the first child of the specified child Page Type. </summary>
        /// <typeparam name="TChildNode"> The Page Type of the child whose children are queried. </typeparam>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="childrenDocumentRetriever"> The <see cref="IChildrenDocumentRetriever"/> to extend. </param>
        /// <param name="parentNodeID"> The <see cref="TreeNode.NodeID"/> of the parent <see cref="TreeNode"/>. </param>
        /// <param name="childQueryFilter"> A delegate method that is used to filter the query that selects the child whose children are retrieved. </param>
        /// <remarks> Implementations of this method may result in the execution of multiple queries. </remarks>
        /// <returns> A query for the children of a child of a node with the given <paramref name="parentNodeID"/>. </returns>
        public static DocumentQuery<TNode> GetChildrenOfChild<TChildNode, TNode>( this IChildrenDocumentRetriever childrenDocumentRetriever, int parentNodeID, Func<DocumentQuery<TChildNode>, DocumentQuery<TChildNode>> childQueryFilter = null )
            where TChildNode : TreeNode, new()
            where TNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( childrenDocumentRetriever );

            var (_, nodes) = childrenDocumentRetriever.GetChildrenOfChild<TChildNode, TNode>( parentNodeID, childQueryFilter );
            return nodes;
        }

        /// <summary> Query documents of any Page Type that are children of the child of the specified child Page Type, as filtered by the <paramref name="childQueryFilter"/>. </summary>
        /// <typeparam name="TChildNode"> The Page Type of the child whose children are queried. </typeparam>
        /// <param name="childrenDocumentRetriever"> The <see cref="IChildrenDocumentRetriever"/> to extend. </param>
        /// <param name="parentNodeID"> The <see cref="TreeNode.NodeID"/> of the parent <see cref="TreeNode"/>. </param>
        /// <param name="childQueryFilter"> A delegate method that is used to filter the query that selects the child whose children are retrieved. </param>
        /// <remarks> Implementations of this method may result in the execution of multiple queries. </remarks>
        /// <returns> A query for the children of a child of a node with the given <paramref name="parentNodeID"/>. </returns>
        public static MultiDocumentQuery GetChildrenOfChild<TChildNode>( this IChildrenDocumentRetriever childrenDocumentRetriever, int parentNodeID, Func<DocumentQuery<TChildNode>, DocumentQuery<TChildNode>> childQueryFilter = null )
            where TChildNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( childrenDocumentRetriever );

            var (_, nodes) = childrenDocumentRetriever.GetChildrenOfChild( parentNodeID, childQueryFilter );
            return nodes;
        }

        private static void ThrowIfRetrieverIsNull( IChildrenDocumentRetriever childrenDocumentRetriever )
        {
            if( childrenDocumentRetriever == null )
            {
                throw new ArgumentNullException( nameof( childrenDocumentRetriever ) );
            }
        }

    }

}
