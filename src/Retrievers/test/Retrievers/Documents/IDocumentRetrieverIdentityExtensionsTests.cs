using System;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Documents;
using CMS.DocumentEngine;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Retrievers.Documents
{

    [TestFixture( Category = "Integration" )]
    [TestOf( typeof( IDocumentRetrieverIdentityExtensions ) )]
    public class IDocumentRetrieverIdentityExtensionsTests : BaseDocumentRetrieverIntegrationTests
    {
        protected override Type RetrieverType => null;
        protected override Type RetrieverInterfaceType => null;

        private IDocumentRetriever CreateDocumentRetriever( )
            => new DocumentRetriever( new MockOptionsSnapshot<DocumentRetrieverOptions>( new DocumentRetrieverOptions() ) );

        protected override void InitContentTree( )
        {
        }

        public override void Retriever_InterfaceMethods_ShouldBeOverridable( )
        {
            // nada.
        }

        #region Get Tests
        [Test]
        public void Get_ByDocumentID_ShouldReturnNode( )
        {
            var node = CreateDocumentRetriever().GetDocument<TreeNode>( RootNode.DocumentID );
            Assert.IsNotNull( node );
            Assert.AreEqual( RootNode.DocumentID, node.DocumentID );
        }

        [Test]
        public void Get_ByDocumentGuid_ShouldReturnNode( )
        {
            var node = CreateDocumentRetriever().GetDocument<TreeNode>( RootNode.DocumentGUID );
            Assert.IsNotNull( node );
            Assert.AreEqual( RootNode.DocumentGUID, node.DocumentGUID );
        }

        [Test]
        public void GetNode_ByNodeID_ShouldReturnNode( )
        {
            var node = CreateDocumentRetriever().GetNode<TreeNode>( RootNode.NodeID );
            Assert.IsNotNull( node );
            Assert.AreEqual( RootNode.NodeID, node.NodeID );
        }

        [Test]
        public void GetNode_ByNodeGuid_ShouldReturnNode( )
        {
            var node = CreateDocumentRetriever().GetNode<TreeNode>( RootNode.NodeGUID );
            Assert.IsNotNull( node );
            Assert.AreEqual( RootNode.NodeGUID, node.NodeGUID );
        }

        [Test]
        public void GetNode_ByNodeAliasPath_ShouldReturnNode( )
        {
            var node = CreateDocumentRetriever().GetNode<TreeNode>( RootNode.NodeAliasPath );
            Assert.IsNotNull( node );
            Assert.AreEqual( RootNode.NodeAliasPath, node.NodeAliasPath );

        }
        #endregion

    }

}
