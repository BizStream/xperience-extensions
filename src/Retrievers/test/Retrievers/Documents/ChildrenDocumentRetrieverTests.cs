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
    [TestOf( typeof( ChildrenDocumentRetriever ) )]
    public class ChildrenDocumentRetrieverTests : BaseDocumentRetrieverIntegrationTests
    {
        #region Fields
        private TreeNode ChildNode1;
        private TreeNode ChildNode2;

        protected override Type RetrieverType { get; } = typeof( ChildrenDocumentRetriever );
        protected override Type RetrieverInterfaceType { get; } = typeof( IChildrenDocumentRetriever );
        #endregion

        private ChildrenDocumentRetriever CreateChildrenDocumentRetriever( IDocumentRetriever documentRetriever = null )
            => new ChildrenDocumentRetriever( documentRetriever ?? CreateDocumentRetriever() );

        protected override void InitContentTree( )
        {
            /*
             * - CMS.Root
             *  - ChildNode1
             *      - descendant 1..5
             *  - ChildNode2
             *      - descendant 1..5
             */

            ChildNode1 = new TestNode { DocumentName = "child node 1." };
            DocumentHelper.InsertDocument( ChildNode1, RootNode );

            foreach(
                var child in Enumerable.Range( 1, 6 )
                    .Select( n => new TestNode { DocumentName = $"descendant node {n}" } )
            )
            {
                DocumentHelper.InsertDocument( child, ChildNode1 );
            }

            ChildNode2 = new TestNode { DocumentName = "child node 2." };
            DocumentHelper.InsertDocument( ChildNode2, RootNode );

            foreach(
                var child in Enumerable.Range( 1, 6 )
                    .Select( n => new TestNode { DocumentName = $"descendant node {n}" } )
            )
            {
                DocumentHelper.InsertDocument( child, ChildNode2 );
            }
        }

        #region GetChildren Tests
        [Test]
        public void GetChildren_DocumentQuery_ShouldReturnNodes( )
            => CreateChildrenDocumentRetriever()
                .GetChildren<TreeNode>( RootNode.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetChildren_DocumentQuery_ShouldReturnNodes_OrderedByOrder( )
        {
            var query = CreateChildrenDocumentRetriever()
                .GetChildren<TreeNode>( RootNode.NodeID );

            query.AssertOrderedBy( nameof( TreeNode.NodeOrder ), OrderDirection.Ascending );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeOrder();
        }

        [Test]
        public void GetChildren_MultiDocumentQuery_ShouldReturnNodes( )
            => CreateChildrenDocumentRetriever()
                .GetChildren( RootNode.NodeID )
                .AssertReturnsNodes();

        [Test]
        public void GetChildren_MultiDocumentQuery_ShouldReturnNodes_OrderedByOrder( )
        {
            var query = CreateChildrenDocumentRetriever()
                .GetChildren( RootNode.NodeID );

            query.AssertOrderedBy( nameof( TreeNode.NodeOrder ), OrderDirection.Ascending );

            var nodes = query.ToList();
            nodes.AssertOrderedByNodeOrder();
        }
        #endregion

        #region GetChildrenOfChild Tests
        [Test]
        public void GetChildrenOfChild_DocumentQuery_ShouldReturnNodes( )
        {
            var (_, children) = CreateChildrenDocumentRetriever()
                .GetChildrenOfChild<TreeNode, TreeNode>( RootNode.NodeID );

            children.AssertReturnsNodes();
        }

        [Test]
        public void GetChildrenOfChild_DocumentQuery_ShouldReturnNodes_OrderedByOrder( )
        {
            var (_, children) = CreateChildrenDocumentRetriever()
                .GetChildrenOfChild<TreeNode, TreeNode>( RootNode.NodeID );

            children.AssertOrderedBy( nameof( TreeNode.NodeOrder ), OrderDirection.Ascending );

            var nodes = children.ToList();
            nodes.AssertOrderedByNodeOrder();
        }

        [Test]
        public void GetChildrenOfChild_DocumentQuery_ShouldReturnChild( )
        {
            var (childNode, _) = CreateChildrenDocumentRetriever()
                .GetChildrenOfChild<TreeNode, TreeNode>( RootNode.NodeID );

            Assert.IsNotNull( childNode );
            Assert.AreEqual( ChildNode1.NodeID, childNode.NodeID, "child nodes do not match." );
        }

        [Test]
        public void GetChildrenOfChild_DocumentQuery_ChildQueryFilter_ShouldFilterChildNode( )
        {
            var (childNode, children) = CreateChildrenDocumentRetriever().GetChildrenOfChild<TreeNode, TreeNode>(
                RootNode.NodeID,
                childQuery => childQuery.WhereGreaterThan( nameof( TreeNode.NodeOrder ), 1 )
            );

            Assert.IsNotNull( childNode, $"{nameof( childNode )} was null." );
            Assert.AreEqual( ChildNode2.NodeID, childNode.NodeID, "child nodes do not match." );

            children.AssertReturnsNodes();
        }

        [Test]
        public void GetChildrenOfChild_DocumentQuery_ChildQueryFilter_ShouldForceSelectNodeID( )
        {
            var (childNode, _) = CreateChildrenDocumentRetriever().GetChildrenOfChild<TreeNode, TreeNode>(
                RootNode.NodeID,
                childQuery => childQuery.Column( nameof( TreeNode.NodeAliasPath ) )
            );

            Assert.IsNotNull( childNode );
            Assert.IsTrue( childNode.ContainsColumn( nameof( TreeNode.NodeID ) ), $"node does not contain '{nameof( TreeNode.NodeID )}' column." );
            Assert.AreEqual( ChildNode1.NodeID, childNode.NodeID );
        }

        [Test]
        public void GetChildrenOfChild_MultiDocumentQuery_ShouldReturnNodes( )
        {
            var (_, children) = CreateChildrenDocumentRetriever()
                .GetChildrenOfChild<TreeNode>( RootNode.NodeID );

            children.AssertReturnsNodes();
        }

        [Test]
        public void GetChildrenOfChild_MultiDocumentQuery_ShouldReturnNodes_OrderedByOrder( )
        {
            var (_, children) = CreateChildrenDocumentRetriever()
                .GetChildrenOfChild<TreeNode>( RootNode.NodeID );

            children.AssertOrderedBy( nameof( TreeNode.NodeOrder ), OrderDirection.Ascending );

            var nodes = children.ToList();
            nodes.AssertOrderedByNodeOrder();
        }

        [Test]
        public void GetChildrenOfChild_MultiDocumentQuery_ShouldReturnChild( )
        {
            var (childNode, _) = CreateChildrenDocumentRetriever()
                .GetChildrenOfChild<TreeNode>( RootNode.NodeID );

            Assert.IsNotNull( childNode );
            Assert.AreEqual( ChildNode1.NodeID, childNode.NodeID, "child nodes do not match." );
        }

        [Test]
        public void GetChildrenOfChild_MultiDocumentQuery_ChildQueryFilter_ShouldFilterChildNode( )
        {
            var (childNode, children) = CreateChildrenDocumentRetriever().GetChildrenOfChild<TreeNode>(
                RootNode.NodeID,
                childQuery => childQuery.WhereGreaterThan( nameof( TreeNode.NodeOrder ), 1 )
            );

            Assert.IsNotNull( childNode, $"{nameof( childNode )} was null." );
            Assert.AreEqual( ChildNode2.NodeID, childNode.NodeID, "child nodes do not match." );

            children.AssertReturnsNodes();
        }

        [Test]
        public void GetChildrenOfChild_MultiDocumentQuery_ChildQueryFilter_ShouldForceSelectNodeID( )
        {
            var (childNode, _) = CreateChildrenDocumentRetriever().GetChildrenOfChild<TreeNode>(
                RootNode.NodeID,
                childQuery => childQuery.Column( nameof( TreeNode.NodeAliasPath ) )
            );

            Assert.IsNotNull( childNode );
            Assert.IsTrue( childNode.ContainsColumn( nameof( TreeNode.NodeID ) ), $"node does not contain '{nameof( TreeNode.NodeID )}' column." );
            Assert.AreEqual( ChildNode1.NodeID, childNode.NodeID );
        }
        #endregion

    }

}
