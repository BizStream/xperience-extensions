using System;
using BizStream.Extensions.Kentico.Xperience.Caching;
using BizStream.Extensions.Kentico.Xperience.Caching.Tests.Models;
using CMS.Base;
using CMS.Helpers;
using CMS.Tests;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.CustomTables
{

    [TestFixture( Category = "Unit" )]
    [TestOf( typeof( CMSCacheDependencyExtensions ) )]
    public partial class CMSCacheDependencyExtensionsTests : UnitTests
    {

        [Test]
        public void OnDocument_ByDocumentID_ShouldConfigureKey( )
        {
            var documentID = 2;
            var keys = new CMSCacheDependency()
                .OnDocument( documentID )
                .CacheKeys;

            Assert.Contains( $"documentid|{documentID}".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNode_ByNodeID_ShouldConfigureKey( )
        {
            var nodeID = 2;
            var keys = new CMSCacheDependency()
                .OnNode( nodeID )
                .CacheKeys;

            Assert.Contains( $"nodeid|{nodeID}".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNode_ByNodeAliasPath_ShouldConfigureKey( [Values( null, "", "en-US" )] string cultureCode )
        {
            var siteName = "BizStream.Test";
            var aliasPath = "/Test";

            var keys = new CMSCacheDependency()
                .OnNode( siteName, aliasPath, cultureCode )
                .CacheKeys;

            Assert.Contains( $"node|{siteName}|{aliasPath}".ToLowerCSafe(), keys );
            if( !string.IsNullOrWhiteSpace( cultureCode ) )
            {
                Assert.Contains( $"node|{siteName}|{aliasPath}|{cultureCode}".ToLowerCSafe(), keys );
            }
        }

        [Test]
        public void OnNode_ByNodeGuid_ShouldConfigureKey( )
        {
            var siteName = "BizStream.Test";
            var nodeGuid = Guid.NewGuid();

            var keys = new CMSCacheDependency()
                .OnNode( siteName, nodeGuid )
                .CacheKeys;

            Assert.Contains( $"nodeguid|{siteName}|{nodeGuid}".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNodeDescendants_ShouldConfigureKey( )
        {
            var siteName = "BizStream.Test";
            var aliasPath = "/Test";

            var keys = new CMSCacheDependency()
                .OnNodeDescendants( siteName, aliasPath )
                .CacheKeys;

            Assert.Contains( $"node|{siteName}|{aliasPath}|childnodes".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNodesOfType_ShouldConfigureKey( )
        {
            var siteName = "BizStream.Test";
            var keys = new CMSCacheDependency()
                .OnNodesOfType<TestNode>( siteName )
                .CacheKeys;

            Assert.Contains( $"nodes|{siteName}|{TestNode.CLASS_NAME}|all".ToLowerCSafe(), keys );
        }

    }

}
