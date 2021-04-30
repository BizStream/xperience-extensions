using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using BizStream.Extensions.Kentico.Xperience.DocumentEngine;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using CMS.DataEngine;
using CMS.DocumentEngine;
using Microsoft.Extensions.Options;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> A class that can query to siblings of Kentico Documents. </summary>
    public class SiblingsDocumentRetriever : ISiblingsDocumentRetriever
    {
        #region Fields
        private const string SiblingNodeParentIDColumnName = "_SiblingNodeParentID";

        private readonly IDocumentRetriever documentRetriever;
        private readonly IOptions<SiblingsDocumentRetrieverOptions> options;
        #endregion

        public SiblingsDocumentRetriever( IDocumentRetriever documentRetriever, IOptionsSnapshot<SiblingsDocumentRetrieverOptions> options )
        {
            this.documentRetriever = documentRetriever;
            this.options = options;
        }

        /// <summary> Modifies the query to filter documents that are siblings to the document with the given <paramref name="nodeID"/>. </summary>
        protected virtual TQuery ApplySiblingFilter<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            var typedQuery = query.GetTypedQuery();
            var nodeQuery = new DataQuery().From( Strings.DocumentTreeViewName )
                .Column( nameof( TreeNode.NodeParentID ) )
                .WhereEquals( nameof( TreeNode.NodeID ), nodeID )
                .TopN( 1 );

            switch( options.Value.FilterMethod )
            {
                case DocumentFilterMethod.InnerJoin:
                    string aliasName = typedQuery.GetQueryAliasName();
                    typedQuery = typedQuery.Source(
                        source => source.Join(
                            nodeQuery.AsSingleColumn( $"{nameof( TreeNode.NodeParentID )} AS {SiblingNodeParentIDColumnName}", true ),
                            "Parent",
                            $"{aliasName}.{nameof( TreeNode.NodeParentID )}",
                            $"Parent.{SiblingNodeParentIDColumnName}"
                        )
                    );
                    break;

                case DocumentFilterMethod.InnerQuery:
                    typedQuery = typedQuery.WhereIn(
                        nameof( TreeNode.NodeParentID ),
                        nodeQuery.AsSingleColumn( nameof( TreeNode.NodeParentID ), true )
                    );

                    break;

                case DocumentFilterMethod.Value:
                    var parentNodeID = nodeQuery.GetListResult<int>()
                        ?.FirstOrDefault();

                    typedQuery = parentNodeID.HasValue
                        ? typedQuery.WhereEquals( nameof( TreeNode.NodeParentID ), parentNodeID.Value )
                        : typedQuery.NoResults();

                    break;

                default:
                    throw new UnsupportedDocumentFilterMethodException( options.Value.FilterMethod );
            }

            return typedQuery.WhereNot( WhereCondition.From( condition => condition.WhereEquals( nameof( TreeNode.NodeID ), nodeID ) ) )
                .ResetOrderBy( nameof( TreeNode.NodeOrder ) );
        }

        /// <inheritdoc />
        public virtual DocumentQuery<TNode> GetSiblings<TNode>( int nodeID )
            where TNode : TreeNode, new()
            => ApplySiblingFilter( documentRetriever.GetDocuments<TNode>(), nodeID );

        /// <inheritdoc />
        public virtual MultiDocumentQuery GetSiblings( int nodeID )
            => ApplySiblingFilter( documentRetriever.GetDocuments(), nodeID );

    }

}
