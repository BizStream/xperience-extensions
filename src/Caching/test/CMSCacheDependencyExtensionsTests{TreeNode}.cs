using System;
using BizStream.Extensions.Kentico.Xperience.Caching.Tests.Models;
using CMS.Base;
using CMS.Helpers;
using CMS.Tests;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Caching.Tests
{

    [TestFixture( Category = "Unit" )]
    [TestOf( typeof( CMSCacheDependencyExtensions ) )]
    public partial class CMSCacheDependencyExtensionsTests : UnitTests
    {

        [Test]
        public void OnDocument_ByDocumentID_ShouldConfigureKey( )
        {
            int documentID = 2;
            string[]? keys = new CMSCacheDependency()
                .OnDocument( documentID )
                .CacheKeys;

            Assert.Contains( $"documentid|{documentID}".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNode_ByNodeID_ShouldConfigureKey( )
        {
            int nodeID = 2;
            string[]? keys = new CMSCacheDependency()
                .OnNode( nodeID )
                .CacheKeys;

            Assert.Contains( $"nodeid|{nodeID}".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNode_ByNodeAliasPath_ShouldConfigureKey( [Values( null, "", "en-US" )] string cultureCode )
        {
            string? siteName = "BizStream.Test";
            string? aliasPath = "/Test";

            string[]? keys = new CMSCacheDependency()
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
            string? siteName = "BizStream.Test";
            Guid nodeGuid = Guid.NewGuid();

            string[]? keys = new CMSCacheDependency()
                .OnNode( siteName, nodeGuid )
                .CacheKeys;

            Assert.Contains( $"nodeguid|{siteName}|{nodeGuid}".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNodeDescendants_ShouldConfigureKey( )
        {
            string? siteName = "BizStream.Test";
            string? aliasPath = "/Test";

            string[]? keys = new CMSCacheDependency()
                .OnNodeDescendants( siteName, aliasPath )
                .CacheKeys;

            Assert.Contains( $"node|{siteName}|{aliasPath}|childnodes".ToLowerCSafe(), keys );
        }

        [Test]
        public void OnNodesOfType_ShouldConfigureKey( )
        {
            string? siteName = "BizStream.Test";
            string[]? keys = new CMSCacheDependency()
                .OnNodesOfType<TestNode>( siteName )
                .CacheKeys;

            Assert.Contains( $"nodes|{siteName}|{TestNode.CLASS_NAME}|all".ToLowerCSafe(), keys );
        }

    }

}
