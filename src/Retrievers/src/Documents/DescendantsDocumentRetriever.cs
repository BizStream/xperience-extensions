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

    /// <summary> A class that can query to descendants of Kentico Documents in the Content Tree. </summary>
    public class DescendantsDocumentRetriever : IDescendantsDocumentRetriever
    {
        #region Fields
        private const string CteName = "Descendants";
        private const string DescendantNodeIDColumnName = "_DescendantNodeID";

        private readonly IDocumentRetriever documentRetriever;
        private readonly IOptions<DescendantsDocumentRetrieverOptions> options;
        #endregion

        public DescendantsDocumentRetriever( IDocumentRetriever documentRetriever, IOptionsSnapshot<DescendantsDocumentRetrieverOptions> options )
        {
            this.documentRetriever = documentRetriever;
            this.options = options;
        }

        /// <summary> Modifies the query to join against a recursive-cte of the given node's descendants. </summary>
        protected virtual TQuery ApplyDescendantsFilter<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            var cteAliasName = "dcs";
            var cteQuery = new DataQuery().From( Strings.DocumentTreeViewName )
                .Column( nameof( TreeNode.NodeID ) )
                .WhereEquals( nameof( TreeNode.NodeParentID ), nodeID )
                .UnionAll(
                    new DataQuery().From( new QuerySourceTable( Strings.DocumentTreeViewName, "Tree" ) )
                        .Column( $"Tree.{nameof( TreeNode.NodeID )}" )
                        .Source(
                            source => source.InnerJoin(
                                new QuerySourceTable( CteName, cteAliasName ),
                                $"Tree.{nameof( TreeNode.NodeParentID )}",
                                $"{cteAliasName}.{nameof( TreeNode.NodeID )}"
                            )
                        )
                );

            var typedQuery = query.GetTypedQuery();
            var filterMethod = options.Value.FilterMethod;
            switch( filterMethod )
            {
                case DocumentFilterMethod.InnerJoin:
                    string aliasName = typedQuery.GetQueryAliasName();
                    typedQuery = typedQuery.WithCte( CteName, cteQuery )
                        .Source(
                            source => source.Join(
                                new DataQuery().From( CteName )
                                    .AsSingleColumn( $"{nameof( TreeNode.NodeID )} AS {DescendantNodeIDColumnName}", true ),
                                cteAliasName,
                                $"{aliasName}.{nameof( TreeNode.NodeID )}",
                                $"{cteAliasName}.{DescendantNodeIDColumnName}"
                            )
                        );

                    break;

                case DocumentFilterMethod.InnerQuery:
                case DocumentFilterMethod.Value:
                    var descendantNodeIDQuery = new DataQuery().From( CteName )
                        .WithCte( CteName, cteQuery )
                        .AsSingleColumn( nameof( TreeNode.NodeID ), true );

                    if( filterMethod == DocumentFilterMethod.Value )
                    {
                        var descendantNodeIDs = descendantNodeIDQuery.GetListResult<int>();
                        typedQuery = descendantNodeIDs.Any()
                            ? typedQuery.WhereIn( nameof( TreeNode.NodeID ), descendantNodeIDs )
                            : typedQuery.NoResults();
                    }
                    else
                    {
                        typedQuery = typedQuery.WhereIn( nameof( TreeNode.NodeID ), descendantNodeIDQuery );
                    }

                    break;

                default:
                    throw new UnsupportedDocumentFilterMethodException( filterMethod );
            }

            return typedQuery.ResetOrderBy( nameof( TreeNode.NodeLevel ), OrderDirection.Ascending );
        }

        /// <inheritdoc />
        public virtual DocumentQuery<TNode> GetDescendants<TNode>( int nodeID )
            where TNode : TreeNode, new()
            => ApplyDescendantsFilter( documentRetriever.GetDocuments<TNode>(), nodeID );

        /// <inheritdoc />
        public virtual MultiDocumentQuery GetDescendants( int nodeID )
            => ApplyDescendantsFilter( documentRetriever.GetDocuments(), nodeID );

    }

}
