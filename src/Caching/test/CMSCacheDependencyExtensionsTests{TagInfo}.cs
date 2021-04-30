using BizStream.Extensions.Kentico.Xperience.Caching;
using CMS.Helpers;
using CMS.Membership;
using CMS.Taxonomy;
using CMS.Tests;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.CustomTables
{

    [TestFixture( Category = "Unit" )]
    [TestOf( typeof( CMSCacheDependencyExtensions ) )]
    public partial class CMSCacheDependencyExtensionsTests : UnitTests
    {

        [Test]
        public void OnTag_ByTagName_ShouldConfigureKey( )
        {
            var tagName = "test";
            var keys = new CMSCacheDependency()
                .OnTag( tagName )
                .CacheKeys;

            Assert.Contains( $"{TagInfo.OBJECT_TYPE}|byname|{tagName}", keys );
        }

        [Test]
        public void OnTags_ShouldConfigureKey( )
        {
            var keys = new CMSCacheDependency()
                .OnTags()
                .CacheKeys;

            Assert.Contains( $"{TagInfo.OBJECT_TYPE}|all", keys );
        }

    }

}
