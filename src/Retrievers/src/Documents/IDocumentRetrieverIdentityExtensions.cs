using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> Extensions to <see cref="IDocumentRetriever"/> that support querying a single nodes or documents by common unique identifies. </summary>
    public static class IDocumentRetrieverIdentityExtensions
    {

        /// <summary> Query a document of the specified Page Type with the given <paramref name="documentID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="documentID"> The ID of the document to retrieve. </param>
        /// <returns> The document of identified by <paramref name="documentID"/>. </returns>
        public static TNode GetDocument<TNode>( this IDocumentRetriever documentRetriever, int documentID )
            where TNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( documentRetriever );
            return documentRetriever.GetDocuments<TNode>()
                .WhereEquals( nameof( TreeNode.DocumentID ), documentID )
                .TopN( 1 )
                .FirstOrDefault();
        }

        /// <summary> Query a document of the specified Page Type with the given <paramref name="documentGuid"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="documentGuid"> The <see cref="Guid"/> of the document to retrieve. </param>
        /// <returns> The document of identified by <paramref name="documentGuid"/>. </returns>
        public static TNode GetDocument<TNode>( this IDocumentRetriever documentRetriever, Guid documentGuid )
            where TNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( documentRetriever );
            return documentRetriever.GetDocuments<TNode>()
                .WhereEquals( nameof( TreeNode.DocumentGUID ), documentGuid )
                .TopN( 1 )
                .FirstOrDefault();
        }

        /// <summary> Query a document for the node with the given <paramref name="nodeID"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> of the document to retrieve. </param>
        /// <returns> A document for the node identified by the given <paramref name="nodeID"/>. </returns>
        public static TNode GetNode<TNode>( this IDocumentRetriever documentRetriever, int nodeID )
            where TNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( documentRetriever );
            return documentRetriever.GetDocuments<TNode>()
                .WhereEquals( nameof( TreeNode.NodeID ), nodeID )
                .TopN( 1 )
                .FirstOrDefault();
        }

        /// <summary> Query a document for the node with the given <paramref name="nodeGuid"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeGuid"> The <see cref="TreeNode.NodeGUID"/> of the document to retrieve. </param>
        /// <returns> A document for the node identified by the given <paramref name="nodeGuid"/>. </returns>
        public static TNode GetNode<TNode>( this IDocumentRetriever documentRetriever, Guid nodeGuid )
            where TNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( documentRetriever );
            return documentRetriever.GetDocuments<TNode>()
                .WhereEquals( nameof( TreeNode.NodeGUID ), nodeGuid )
                .TopN( 1 )
                .FirstOrDefault();
        }

        /// <summary> Query a document for the node with the given <paramref name="nodeAliasPath"/>. </summary>
        /// <typeparam name="TNode"> The Page Type of documents to query. </typeparam>
        /// <param name="nodeAliasPath"> The <see cref="TreeNode.NodeAliasPath"/> of the document to retrieve. </param>
        /// <returns> A document for the node identified by the given <paramref name="nodeAliasPath"/>. </returns>
        public static TNode GetNode<TNode>( this IDocumentRetriever documentRetriever, string nodeAliasPath )
            where TNode : TreeNode, new()
        {
            ThrowIfRetrieverIsNull( documentRetriever );
            return documentRetriever.GetDocuments<TNode>()
                .WhereEquals( nameof( TreeNode.NodeAliasPath ), nodeAliasPath )
                .TopN( 1 )
                .FirstOrDefault();
        }

        private static void ThrowIfRetrieverIsNull( IDocumentRetriever documentRetriever )
        {
            if( documentRetriever == null )
            {
                throw new ArgumentNullException( nameof( documentRetriever ) );
            }
        }

    }

}
