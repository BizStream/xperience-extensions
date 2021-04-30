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
    [TestOf( typeof( DocumentRetriever ) )]
    public class DocumentRetrieverTests : BaseDocumentRetrieverIntegrationTests
    {
        protected override Type RetrieverType { get; } = typeof( DocumentRetriever );
        protected override Type RetrieverInterfaceType { get; } = typeof( IDocumentRetriever );

        protected override void InitContentTree( )
        {
            /*
             * - CMS.Root
             *  - test node 1..5
             */

            var nodes = Enumerable.Range( 1, 6 )
                .Select( n => new TestNode { DocumentName = $"test node {n}" } );

            foreach( var node in nodes )
            {
                DocumentHelper.InsertDocument( node, RootNode );
            }
        }

        #region Options Tests
        [Test]
        public void Options_LatestVersion_DocumentQuery_ShouldReturnLatestVersion( )
        {
            var options = new DocumentRetrieverOptions
            {
                Version = DocumentVersion.Latest
            };

            var node = CreateDocumentRetriever( options ).GetDocuments<TreeNode>()
                .TopN( 1 )
                .FirstOrDefault();

            Assert.IsNotNull( node, "Node was not returned." );
            Assert.IsTrue( node.IsLastVersion, "Latest version was not retrieved." );
        }

        [Test]
        public void Options_PublishedVersion_DocumentQuery_ShouldReturnPublishedVersion( )
        {
            var options = new DocumentRetrieverOptions
            {
                Version = DocumentVersion.Published
            };

            var node = CreateDocumentRetriever( options ).GetDocuments<TreeNode>()
                .TopN( 1 )
                .FirstOrDefault();

            Assert.IsNotNull( node, "Node was not returned." );
            Assert.IsTrue( node.IsPublished, "Published version was not retrieved." );
        }

        [Test]
        public void Options_LatestVersion_MultiDocumentQuery_ShouldReturnLatestVersion( )
        {
            var options = new DocumentRetrieverOptions
            {
                Version = DocumentVersion.Latest
            };

            var node = CreateDocumentRetriever( options ).GetDocuments()
                .TopN( 1 )
                .FirstOrDefault();

            Assert.IsNotNull( node, "Node was not returned." );
            Assert.IsTrue( node.IsLastVersion, "Latest version was not retrieved." );
        }

        [Test]
        public void Options_PublishedVersion_MultiDocumentQuery_ShouldReturnPublishedVersion( )
        {
            var options = new DocumentRetrieverOptions
            {
                Version = DocumentVersion.Published
            };

            var node = CreateDocumentRetriever( options ).GetDocuments()
                .TopN( 1 )
                .FirstOrDefault();

            Assert.IsNotNull( node, "Node was not returned." );
            Assert.IsTrue( node.IsPublished, "Published version was not retrieved." );
        }
        #endregion

        #region Get Tests
        [Test]
        public void Get_DocumentQuery_ShouldReturnNodes( )
            => CreateDocumentRetriever().GetDocuments<TreeNode>()
                .AssertReturnsNodes();

        [Test]
        public void Get_MultiDocumentQuery_ShouldReturnNodes( )
            => CreateDocumentRetriever().GetDocuments()
                .AssertReturnsNodes();
        #endregion

    }

}
