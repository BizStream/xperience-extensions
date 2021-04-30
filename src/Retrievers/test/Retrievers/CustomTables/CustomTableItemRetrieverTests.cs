using System;
using System.Collections.Generic;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.CustomTables;
using BizStream.Extensions.Kentico.Xperience.Retrievers.CustomTables;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Extensions;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Models;
using CMS.CustomTables;
using CMS.DataEngine;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.CustomTables
{

    [TestFixture( Category = "Integration" )]
    [TestOf( typeof( CustomTableItemRetriever ) )]
    public class CustomTableItemRetrieverTests : BaseCustomTableItemRetrieverTests
    {
        protected override Type RetrieverType { get; } = typeof( CustomTableItemRetriever );
        protected override Type RetrieverInterfaceType { get; } = typeof( ICustomTableItemRetriever );

        private void AssertItemsAreOrdered( IEnumerable<TestItem> items, OrderDirection direction = OrderDirection.Default )
        {
            var ordered = items.Zip( items.Skip( 1 ), ( a, b ) => new { a, b } )
                .All(
                    p => direction == OrderDirection.Descending
                        ? p.a.ItemOrder > p.b.ItemOrder
                        : p.a.ItemOrder < p.b.ItemOrder
                );

            Assert.IsTrue( ordered, "item are not ordered by ItemOrder." );
        }

        protected override void InitCustomTableItems( )
        {
            foreach(
                var item in Enumerable.Range( 1, 6 )
                    .Select( n => new TestItem { Value = $"item {n}", ItemOrder = n - 1 } )
            )
            {
                CustomTableItemProvider.SetItem( item );
            }
        }

        [Test]
        public void Get_ShouldReturnItems( )
            => CreateCustomTableItemRetriever()
                .GetItems<TestItem>()
                .AssertReturnsItems();

        [Test]
        public void Get_ShouldReturnItems_OrderedByOrder( )
        {
            var query = CreateCustomTableItemRetriever().GetItems<TestItem>();
            query.AssertOrderedBy( nameof( CustomTableItem.ItemOrder ) );

            var items = query.ToList();
            AssertItemsAreOrdered( items );
        }

    }

}
