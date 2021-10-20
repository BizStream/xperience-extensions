using BizStream.Extensions.Kentico.Xperience.Caching.Tests.Models;
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
        public void OnCustomTableItem_ByItemID_ShouldConfigureKey( )
        {
            int itemID = 0;
            string[]? keys = new CMSCacheDependency()
                .OnCustomTableItem<TestItem>( itemID )
                .CacheKeys;

            Assert.Contains( $"customtableitem.{TestItem.CLASS_NAME}|byid|{itemID}".ToLower(), keys );
        }

        [Test]
        public void OnCustomTableItems_ShouldConfigureKey( )
        {
            string[]? keys = new CMSCacheDependency()
                .OnCustomTableItems<TestItem>()
                .CacheKeys;

            Assert.Contains( $"customtableitem.{TestItem.CLASS_NAME}|all".ToLower(), keys );
        }

    }

}
