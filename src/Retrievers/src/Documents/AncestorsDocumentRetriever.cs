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

    /// <summary> A class that can query the ancestors of Kentico Documents within the Content Tree. </summary>
    public class AncestorsDocumentRetriever : IAncestorsDocumentRetriever
    {
        #region Fields
        private const string CteName = "Ancestors";
        private const string AncestorNodeIDColumnName = "_DescendantNodeID";
        private readonly string[] cteColumns = new[] { "NodeID", "NodeParentID" };

        private readonly IDocumentRetriever documentRetriever;
        private readonly IOptions<AncestorsDocumentRetrieverOptions> options;
        #endregion

        public AncestorsDocumentRetriever( IDocumentRetriever documentRetriever, IOptionsSnapshot<AncestorsDocumentRetrieverOptions> options )
        {
            this.documentRetriever = documentRetriever;
            this.options = options;
        }

        /// <summary> Modifies the query to filter documents to ancestors of the document with the given <paramref name="nodeID"/>. </summary>
        protected virtual TQuery ApplyAncestorsFilter<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query, int nodeID )
            where TQuery : IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            var cteAliasName = "acs";
            var cteQuery = new DataQuery().From( Strings.DocumentTreeViewName )
                .Columns( cteColumns )
                .WhereEquals(
                    nameof( TreeNode.NodeID ),
                    new DataQuery().From( Strings.DocumentTreeViewName )
                        .Column( nameof( TreeNode.NodeParentID ) )
                        .WhereEquals( nameof( TreeNode.NodeID ), nodeID )
                        .TopN( 1 )
                        .AsSingleColumn( nameof( TreeNode.NodeParentID ), true )
                )
                .TopN( 1 )
                .UnionAll(
                    new DataQuery().From( new QuerySourceTable( Strings.DocumentTreeViewName, "Tree" ) )
                        .Columns( cteColumns.Select( column => $"Tree.{column}" ) )
                        .Source(
                            source => source.InnerJoin(
                                new QuerySourceTable( CteName, cteAliasName ),
                                $"Tree.{nameof( TreeNode.NodeID )}",
                                $"{cteAliasName}.{nameof( TreeNode.NodeParentID )}"
                            )
                        )
                );

            var typedQuery = query.GetTypedQuery();
            var filterMethod = options.Value.FilterMethod;
            switch( filterMethod )
            {
                case DocumentFilterMethod.InnerJoin:
                    string aliasName = typedQuery.GetQueryAliasName();
                    typedQuery = typedQuery.WithCte( CteName, cteQuery, cteColumns )
                        .Source(
                            source => source.Join(
                                new DataQuery().From( CteName )
                                    .AsSingleColumn( $"{nameof( TreeNode.NodeID )} as {AncestorNodeIDColumnName}" ),
                                cteAliasName,
                                $"{aliasName}.{nameof( TreeNode.NodeID )}",
                                $"{cteAliasName}.{AncestorNodeIDColumnName}",
                                new WhereCondition( $"{aliasName}.{nameof( TreeNode.NodeLevel )}", QueryOperator.GreaterOrEquals, 1 ),
                                JoinTypeEnum.Inner
                            )
                        );
                    break;

                case DocumentFilterMethod.InnerQuery:
                case DocumentFilterMethod.Value:
                    var ancestorNodeIDQuery = new DataQuery().From( CteName )
                        .WithCte( CteName, cteQuery, cteColumns )
                        .AsSingleColumn( nameof( TreeNode.NodeID ), true );

                    if( filterMethod == DocumentFilterMethod.Value )
                    {
                        var ancestorNodeIDs = ancestorNodeIDQuery.GetListResult<int>();
                        typedQuery = ancestorNodeIDs.Any()
                            ? typedQuery.WhereIn( nameof( TreeNode.NodeID ), ancestorNodeIDs )
                            : typedQuery.NoResults();
                    }
                    else
                    {
                        typedQuery = typedQuery.WhereIn( nameof( TreeNode.NodeID ), ancestorNodeIDQuery );
                    }

                    break;

                default:
                    throw new UnsupportedDocumentFilterMethodException( options.Value.FilterMethod );
            }

            return typedQuery.ResetOrderBy( nameof( TreeNode.NodeLevel ), OrderDirection.Descending );
        }

        /// <inheritdoc />
        public virtual TNode GetAncestor<TNode>( int nodeID )
            where TNode : TreeNode, new()
            => GetAncestors<TNode>( nodeID ).TopN( 1 )
                .FirstOrDefault();

        /// <inheritdoc />
        public virtual DocumentQuery<TNode> GetAncestors<TNode>( int nodeID )
            where TNode : TreeNode, new()
            => ApplyAncestorsFilter( documentRetriever.GetDocuments<TNode>(), nodeID );

        /// <inheritdoc />
        public virtual MultiDocumentQuery GetAncestors( int nodeID )
            => ApplyAncestorsFilter( documentRetriever.GetDocuments(), nodeID );

    }
}
