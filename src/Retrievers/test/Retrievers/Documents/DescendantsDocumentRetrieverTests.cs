﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
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
    [TestOf( typeof( DescendantsDocumentRetriever ) )]
    public class DescendantsDocumentRetrieverTests : BaseDocumentRetrieverIntegrationTests
    {
        protected override Type RetrieverType { get; } = typeof( DescendantsDocumentRetriever );
        protected override Type RetrieverInterfaceType { get; } = typeof( IDescendantsDocumentRetriever );

        private DescendantsDocumentRetriever CreateDescendantDocumentRetriever( IDocumentRetriever documentRetriever = null, DescendantsDocumentRetrieverOptions options = null )
            => new DescendantsDocumentRetriever(
                documentRetriever ?? CreateDocumentRetriever(),
                new MockOptionsSnapshot<DescendantsDocumentRetrieverOptions>( options ?? new DescendantsDocumentRetrieverOptions() )
            );

        protected override void InitContentTree( )
        {
            var _ = Enumerable.Range( 1, 6 )
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
                Assert.IsFalse( Regex.IsMatch( query.WhereCondition, @"(\[NodeID\] IN \( (\d,)+)" ), "Where clause does not contain '[NodeID] IN ( <NODEIDS>'." );
            }
        }

        private void AssertFiltersByJoin<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
        {
            var sourceExpression = query.QuerySource?.SourceExpression;
            Assert.IsTrue( sourceExpression?.Contains( "INNER JOIN ( SELECT NodeID AS [_DescendantNodeID]" ), "Query source does not contain an inner join that starts with 'INNER JOIN Descendants'." );
        }

        private void AssertFiltersByQuery<TQuery, TNode>( IDocumentQuery<TQuery, TNode> query )
            where TQuery : IDocumentQuery<TQuery, TNode>
            where TNode : TreeNode, new()
            => Assert.IsTrue( query.WhereCondition?.Contains( "[NodeID] IN (" ), "Where clause does not contain an inner query that starts with '[NodeID] IN ('." );

        [Test]
        public void Options_FilterMethod_DocumentQuery_ShouldFilterByMethod( [Values] DocumentFilterMethod filterMethod )
        {
            var retriever = CreateDescendantDocumentRetriever( null, new DescendantsDocumentRetrieverOptions { FilterMethod = filterMethod } );

            var query = retriever.GetDescendants<TreeNode>( RootNode.NodeID );
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
            var retriever = CreateDescendantDocumentRetriever( null, new DescendantsDocumentRetrieverOptions { FilterMethod = filterMethod } );

            var query = retriever.GetDescendants( RootNode.NodeID );
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
        public void GetDescendants_DocumentQuery_ShouldReturnNodes( [Values] DocumentFilterMethod filterMethod )
            => CreateDescendantDocumentRetriever( null, new DescendantsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetDescendants<TreeNode>( RootNode.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetDescendants_DocumentQuery_ShouldReturnNodes_OrderedByLevel( [Values] DocumentFilterMethod filterMethod )
        {
            var query = CreateDescendantDocumentRetriever( null, new DescendantsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetDescendants<TreeNode>( RootNode.NodeID );

            query.AssertOrderedBy( nameof( TreeNode.NodeLevel ), OrderDirection.Ascending );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeLevel( OrderDirection.Ascending );
        }

        [Test]
        public void GetDescendants_MultiDocumentQuery_ShouldReturnNodes( [Values] DocumentFilterMethod filterMethod )
            => CreateDescendantDocumentRetriever( null, new DescendantsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetDescendants( RootNode.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetDescendants_MultiDocumentQuery_ShouldReturnNodes_OrderedByLevel( [Values] DocumentFilterMethod filterMethod )
        {
            var query = CreateDescendantDocumentRetriever( null, new DescendantsDocumentRetrieverOptions { FilterMethod = filterMethod } )
                .GetDescendants( RootNode.NodeID );

            query.AssertOrderedBy( nameof( TreeNode.NodeLevel ), OrderDirection.Ascending );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeLevel( OrderDirection.Ascending );
        }
        #endregion

    }

}
