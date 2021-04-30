using System;
using System.Collections.Generic;
using System.Linq;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Extensions
{

    internal static class AssertionExtensions
    {

        /// <summary> Asserts that the <see cref="IDataQuerySettings.OrderByColumns"/> contains the given <paramref name="columnName"/>. </summary>
        public static void AssertOrderedBy( this IDataQuery query, string columnName, OrderDirection direction = OrderDirection.Default )
        {
            var directionValue = direction == OrderDirection.Ascending
                ? "ASC"
                : direction == OrderDirection.Descending
                    ? "DESC"
                    : string.Empty;

            var column = new OrderByColumn( columnName, direction ).ToString();
            Assert.Contains(
                column,
                ( query.OrderByColumns ?? string.Empty ).Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries )
                    .Select( orderBy => orderBy.Trim() )
                    .ToArray(),
                $"'{column}' is not included in the '{nameof( query.OrderByColumns )}'."
            );
        }

        public static void AssertOrderedByNodeLevel( this IEnumerable<TreeNode> nodes, OrderDirection direction = OrderDirection.Ascending )
        {
            var ordered = nodes.Zip( nodes.Skip( 1 ), ( a, b ) => new { a, b } )
                .All( p =>
                    direction == OrderDirection.Descending
                        ? p.a.NodeLevel > p.b.NodeLevel
                        : p.a.NodeLevel < p.b.NodeLevel
                );

            Assert.IsTrue( ordered, "nodes are not ordered by NodeLevel." );
        }

        public static void AssertOrderedByNodeOrder( this IEnumerable<TreeNode> nodes, OrderDirection direction = OrderDirection.Ascending )
        {
            var ordered = nodes.Zip( nodes.Skip( 1 ), ( a, b ) => new { a, b } )
                .All(
                    p => direction == OrderDirection.Descending
                        ? p.a.NodeOrder > p.b.NodeOrder
                        : p.a.NodeOrder < p.b.NodeOrder
                );

            Assert.IsTrue( ordered, "nodes are not ordered by NodeOrder." );
        }

        public static void AssertReturnsItems( this IQueryable<CustomTableItem> query )
            => Assert.IsTrue( query.Any(), "query does not return any items." );

        public static void AssertReturnsNodes( this IQueryable<TreeNode> query )
            => Assert.IsTrue( query.Any(), "query does not return any nodes." );

    }

}
