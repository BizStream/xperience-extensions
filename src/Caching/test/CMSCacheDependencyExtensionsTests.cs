using CMS.Helpers;
using CMS.Membership;
using CMS.Tests;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Caching.Tests
{

    [TestFixture( Category = "Unit" )]
    [TestOf( typeof( CMSCacheDependencyExtensions ) )]
    public partial class CMSCacheDependencyExtensionsTests : UnitTests
    {

        [Test]
        public void OnObject_ByCodeName_ShouldConfigureKey( )
        {
            string? codeName = "administrator";
            string[]? keys = new CMSCacheDependency()
                .OnObject<UserInfo>( codeName )
                .CacheKeys;

            Assert.Contains( $"cms.user|byname|{codeName}", keys );
        }

        [Test]
        public void OnObjectsOfType_ShouldConfigureKey( )
        {
            string[]? keys = new CMSCacheDependency()
                .OnObjectsOfType<UserInfo>()
                .CacheKeys;

            Assert.Contains( $"cms.user|all", keys );
        }

    }

}
