using CMS.DataEngine;
using CMS.Tests;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine.Tests
{

    [TestFixture( Category = "Unit" )]
    [TestOf( typeof( IDataQueryExtensions ) )]
    public class IDataQueryExtensionsTests : UnitTests
    {

        [Test]
        public void ResetOrderBy_ShouldSetOrderByColumns(
            [Values( "TestID", "TestName", "TestOrder" )] string columnName,
            [Values( OrderDirection.Ascending, OrderDirection.Descending )] OrderDirection direction
        )
        {
            var orderByColumn = new DataQuery()
                .ResetOrderBy( columnName, direction )
                .OrderByColumns;

            Assert.AreEqual(
                $"[{columnName}] {( direction == OrderDirection.Ascending ? "ASC" : "DESC" )}",
                orderByColumn
            );
        }

        [Test]
        public void WithCte_ShouldCreateCteRegionInQueryBefore( )
        {
            var inner = new DataQuery().From( "Table T" );

            var query = new DataQuery().From( "OtherTable OT" )
                .WithCte( nameof( inner ), inner );

            Assert.That( query.Parameters.QueryBefore.Contains( "--region: CTEs" ), $"{nameof( query.Parameters.QueryBefore )} does not contain the starting CTE Region comment." );
            Assert.That( query.Parameters.QueryBefore.Contains( "--endregion: CTEs" ), $"{nameof( query.Parameters.QueryBefore )} does not contain the ending CTE Region comment." );
        }

    }

}
