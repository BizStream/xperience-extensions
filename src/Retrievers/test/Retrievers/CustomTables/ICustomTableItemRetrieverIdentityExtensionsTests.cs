using System;
using BizStream.Extensions.Kentico.Xperience.Retrievers.CustomTables;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Models;
using CMS.CustomTables;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.CustomTables
{

    [TestFixture( Category = "Integration" )]
    [TestOf( typeof( ICustomTableItemRetrieverIdentityExtensions ) )]
    public class ICustomTableItemRetrieverIdentityExtensionsTests : BaseCustomTableItemRetrieverTests
    {
        #region Fields
        private TestItem TestItem;

        protected override Type RetrieverType => null;
        protected override Type RetrieverInterfaceType => null;
        #endregion

        protected override void InitCustomTableItems( )
        {
            TestItem = new TestItem
            {
                Value = "test item"
            };

            CustomTableItemProvider.SetItem( TestItem );
        }

        public override void Retriever_InterfaceMethods_ShouldBeOverridable( )
        {
            // nada.
        }

        [Test]
        public void Get_ByID_ShouldReturnItem( )
        {
            var item = CreateCustomTableItemRetriever()
                .GetItem<TestItem>( TestItem.ItemID );

            Assert.IsNotNull( item, "item was not returned." );
            Assert.AreEqual( TestItem.ItemID, item.ItemID );
            Assert.AreEqual( TestItem.Value, item.Value );
        }

        [Test]
        public void Get_ByGuid_ShouldReturnItem( )
        {
            var item = CreateCustomTableItemRetriever()
                .GetItem<TestItem>( TestItem.ItemGUID );

            Assert.IsNotNull( item, "item was not returned." );
            Assert.AreEqual( TestItem.ItemGUID, item.ItemGUID );
            Assert.AreEqual( TestItem.Value, item.Value );
        }

    }

}
