using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Documents;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Extensions;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Models;
using CMS.DataEngine;
using CMS.DocumentEngine;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Retrievers.Documents
{

    [TestFixture( Category = "Integration" )]
    [TestOf( typeof( AncestorsDocumentRetriever ) )]
    public class AncestorsDocumentRetrieverTests : BaseDocumentRetrieverIntegrationTests
    {
        #region Fields
        private TreeNode DeepNestedNode;

        protected override Type RetrieverType { get; } = typeof( AncestorsDocumentRetriever );
        protected override Type RetrieverInterfaceType { get; } = typeof( IAncestorsDocumentRetriever );
        #endregion

        private AncestorsDocumentRetriever CreateAncestorsDocumentRetriever( IDocumentRetriever documentRetriever = null, AncestorsDocumentRetrieverOptions options = null )
            => new AncestorsDocumentRetriever(
                documentRetriever ?? CreateDocumentRetriever(),
                new MockOptionsSnapshot<AncestorsDocumentRetrieverOptions>( options ?? new AncestorsDocumentRetrieverOptions() )
            );

        protected override void InitContentTree( )
        {
            DeepNestedNode = Enumerable.Range( 1, 6 )
                .Select( n => new TestNode { DocumentName = $"test node {n}" } )
                .Aggregate(
                    RootNode,
                    ( parent, node ) =>
                    {
                        DocumentHelper.InsertDocument( node, parent );
                        return node;
                    }
                );
        }

        #region Options Tests
        private void AssertFiltersByValue<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : WhereConditionBase<TQuery>, IDocumentQuery<TQuery, TNode>, new()
            where TNode : TreeNode, new()
        {
            if( !query.GetTypedQuery().ReturnsNoResults )
            {
                Assert.IsTrue( query.WhereCondition?.Contains( "[NodeID] IN (" ), "Where clause does not contain '[NodeID] IN ('." );
            }
        }

        private void AssertFiltersByJoin<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
        {
            var sourceExpression = query.QuerySource?.SourceExpression;
            Assert.IsTrue( sourceExpression.Contains( "INNER JOIN ( SELECT NodeID AS [_DescendantNodeID]" ), "Query source does not contain an inner join that starts with 'INNER JOIN Ancestors'." );
        }

        private void AssertFiltersByQuery<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
            => Assert.IsTrue( query.WhereCondition?.Contains( "[NodeID] IN (" ), "Where clause does not contain an inner query that starts with '[NodeID] IN ('." );

        [Test]
        public void Options_FilterMethod_DocumentQuery_ShouldFilterByMethod( [Values] DocumentFilterMethod filterMethod )
        {
            var retriever = CreateAncestorsDocumentRetriever( null, new AncestorsDocumentRetrieverOptions { FilterMethod = filterMethod } );

            var query = retriever.GetAncestors<TreeNode>( DeepNestedNode.NodeID );
            if( filterMethod == DocumentFilterMethod.InnerJoin )
            {
                AssertFiltersByJoin( query );
            }

            if( filterMethod == DocumentFilterMethod.InnerQuery )
            {
                AssertFiltersByQuery( query );
            }

            if( filterMethod == DocumentFilterMethod.Value )
            {
                AssertFiltersByValue( query );
            }
        }

        [Test]
        public void Options_FilterMethod_MultiDocumentQuery_ShouldFilterByMethod( [Values] DocumentFilterMethod filterMethod )
        {
            var retriever = CreateAncestorsDocumentRetriever( null, new AncestorsDocumentRetrieverOptions { FilterMethod = filterMethod } );

            var query = retriever.GetAncestors( DeepNestedNode.NodeID );
            if( filterMethod == DocumentFilterMethod.InnerJoin )
            {
                AssertFiltersByJoin( query );
            }

            if( filterMethod == DocumentFilterMethod.InnerQuery )
            {
                AssertFiltersByQuery( query );
            }

            if( filterMethod == DocumentFilterMethod.Value )
            {
                AssertFiltersByValue( query );
            }
        }
        #endregion

        #region GetAncestors Tests
        [Test]
        public void GetAncestors_DocumentQuery_ShouldReturnNodes( [Values] DocumentFilterMethod filterMethod )
        {
            CreateAncestorsDocumentRetriever( null, new AncestorsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetAncestors<TreeNode>( DeepNestedNode.NodeID )
                .AssertReturnsNodes();
        }

        [Test]
        public void GetAncestors_DocumentQuery_ShouldReturnNodes_OrderedByLevel( [Values] DocumentFilterMethod filterMethod )
        {
            var query = CreateAncestorsDocumentRetriever( null, new AncestorsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetAncestors<TreeNode>( DeepNestedNode.NodeID );

            query.AssertOrderedBy( nameof( TreeNode.NodeLevel ), OrderDirection.Descending );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeLevel( OrderDirection.Descending );
        }

        [Test]
        public void GetAncestors_MultiDocumentQuery_ShouldReturnNodes( [Values] DocumentFilterMethod filterMethod )
            => CreateAncestorsDocumentRetriever( null, new AncestorsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetAncestors( DeepNestedNode.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetAncestors_MultiDocumentQuery_ShouldReturnNodes_OrderedByLevel( [Values] DocumentFilterMethod filterMethod )
        {
            var query = CreateAncestorsDocumentRetriever( null, new AncestorsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetAncestors( DeepNestedNode.NodeID );

            query.AssertOrderedBy( nameof( TreeNode.NodeLevel ), OrderDirection.Descending );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeLevel( OrderDirection.Descending );
        }
        #endregion

    }

}
