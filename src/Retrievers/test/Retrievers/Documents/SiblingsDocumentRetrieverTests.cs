using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Documents;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Extensions;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Models;
using CMS.DocumentEngine;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Retrievers.Documents
{

    [TestFixture( Category = "Integration" )]
    [TestOf( typeof( SiblingsDocumentRetriever ) )]
    public class SiblingsDocumentRetrieverTests : BaseDocumentRetrieverIntegrationTests
    {
        #region Fields
        private TreeNode ChildNode1;

        protected override Type RetrieverType { get; } = typeof( SiblingsDocumentRetriever );
        protected override Type RetrieverInterfaceType { get; } = typeof( ISiblingsDocumentRetriever );
        #endregion

        private SiblingsDocumentRetriever CreateSiblingsDocumentRetriever( IDocumentRetriever documentRetriever = null, SiblingsDocumentRetrieverOptions options = null )
            => new SiblingsDocumentRetriever(
                documentRetriever ?? CreateDocumentRetriever(),
                new MockOptionsSnapshot<SiblingsDocumentRetrieverOptions>( options ?? new SiblingsDocumentRetrieverOptions() )
            );

        protected override void InitContentTree( )
        {
            var nodes = Enumerable.Range( 1, 6 )
                .Select( n => new TestNode { DocumentName = $"test node {n}" } )
                .ToList();

            foreach( var node in nodes )
            {
                DocumentHelper.InsertDocument( node, RootNode );
            }

            ChildNode1 = nodes.First();
        }

        #region Options Tests
        private void AssertFiltersByValue<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
        {
            var parameter = query.Parameters[ nameof( TreeNode.NodeParentID ) ];

            Assert.IsNotNull( parameter, "'NodeParentID' parameter was not defined." );
            Assert.AreEqual( RootNode.NodeID, parameter.Value, "Parent NodeID was not correct." );
        }

        private void AssertFiltersByJoin<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
        {
            var sourceExpression = query.QuerySource?.SourceExpression;
            Assert.IsTrue( sourceExpression?.Contains( "INNER JOIN ( SELECT TOP 1 NodeParentID AS [_SiblingNodeParentID]" ), "Query source does not contain an inner join that starts with '(SELECT TOP 1 [NodeParentID]'." );
        }

        private void AssertFiltersByQuery<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
            => Assert.IsTrue( query.WhereCondition?.Contains( "[NodeParentID] IN (" ), "Where clause does not contain an inner query that starts with '[NodeParentID] IN ('." );

        [Test]
        public void Options_FilterMethod_DocumentQuery_ShouldFilterByMethod( [Values] DocumentFilterMethod filterMethod )
        {
            var retriever = CreateSiblingsDocumentRetriever( null, new SiblingsDocumentRetrieverOptions { FilterMethod = filterMethod } );

            var query = retriever.GetSiblings<TreeNode>( ChildNode1.NodeID );
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
            var retriever = CreateSiblingsDocumentRetriever( null, new SiblingsDocumentRetrieverOptions { FilterMethod = filterMethod } );

            var query = retriever.GetSiblings( ChildNode1.NodeID );
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

        #region GetSiblings Tests
        private void AssertOrderedByNodeOrder( IDocumentQuery query )
            => query.AssertOrderedBy( nameof( TreeNode.NodeOrder ) );

        [Test]
        public void GetSiblings_DocumentQuery_ShouldReturnNodes( [Values] DocumentFilterMethod filterMethod )
            => CreateSiblingsDocumentRetriever( null, new SiblingsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetSiblings<TreeNode>( ChildNode1.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetSiblings_DocumentQuery_ShouldReturnNodes_OrderedByOrder( [Values] DocumentFilterMethod filterMethod )
        {
            var query = CreateSiblingsDocumentRetriever( null, new SiblingsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetSiblings<TreeNode>( ChildNode1.NodeID );

            AssertOrderedByNodeOrder( query );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeOrder();
        }

        [Test]
        public void GetSiblings_MultiDocumentQuery_ShouldReturnNodes( [Values] DocumentFilterMethod filterMethod )
            => CreateSiblingsDocumentRetriever( null, new SiblingsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetSiblings( ChildNode1.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetSiblings_MultiDocumentQuery_ShouldReturnNodes_OrderedByOrder( [Values] DocumentFilterMethod filterMethod )
        {
            var query = CreateSiblingsDocumentRetriever( null, new SiblingsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetSiblings( ChildNode1.NodeID );
            AssertOrderedByNodeOrder( query );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeOrder();
        }
        #endregion

    }

}
