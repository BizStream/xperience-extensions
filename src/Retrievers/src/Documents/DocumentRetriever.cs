using System;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using CMS.DocumentEngine;
using Microsoft.Extensions.Options;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> A class that can query Kentico Documents. </summary>
    public class DocumentRetriever : IDocumentRetriever
    {
        #region Fields
        private readonly IOptions<DocumentRetrieverOptions> options;
        #endregion

        public DocumentRetriever( IOptionsSnapshot<DocumentRetrieverOptions> options )
            => this.options = options;

        /// <summary> Modifies the query based on the <see cref="DocumentRetrieverOptions"/>. </summary>
        /// <returns> The modified query. </returns>
        protected virtual TQuery ApplyOptions<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            var optionsValue = options.Value;

            var typedQuery = query.GetTypedQuery()
                .CheckPermissions( optionsValue.CheckPermissions );

            if( !string.IsNullOrWhiteSpace( optionsValue.CultureCode ) )
            {
                typedQuery = typedQuery.Culture( optionsValue.CultureCode );
            }

            if( optionsValue.Version == DocumentVersion.Latest )
            {
                typedQuery = typedQuery.LatestVersion( true )
                    .Published( false );
            }
            else if( optionsValue.Version == DocumentVersion.Published )
            {
                typedQuery = typedQuery.LatestVersion( true )
                    .Published( true );
            }

            if( optionsValue.SiteID.HasValue )
            {
                typedQuery = typedQuery.WhereEquals( nameof( TreeNode.NodeSiteID ), optionsValue.SiteID );
            }

            return typedQuery;
        }

        /// <inheritdoc />
        public virtual DocumentQuery<TNode> GetDocuments<TNode>( )
            where TNode : TreeNode, new()
            => ApplyOptions( DocumentHelper.GetDocuments<TNode>() );

        /// <inheritdoc />
        public virtual MultiDocumentQuery GetDocuments( )
            => ApplyOptions( DocumentHelper.GetDocuments() );

    }

}
