using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine
{

    /// <summary> Extensions to <seealso cref="IDataQuery{TQuery}"/>. </summary>
    public static class IDataQueryExtensions
    {
        #region Fields
        private const string CteRegionEnd = "--endregion: CTEs";
        private const string CteRegionStart = "--region: CTEs";
        private const string CteTemplate = CteWithKeyword + " {0} {1} AS ( {2} )";
        private const string CteWithKeyword = ";WITH";
        #endregion

        /// <summary> Asynchronously executes the given <paramref name="query"/>, returning the first result. </summary>
        /// <param name="query"> The query to execute. </param>
        /// <remarks> This method calls <see cref="IDataQuerySettings{TQuery}.TopN(int)"/> on the given <paramref name="query"/> to ensure only a single record is returned from the DB. </remarks>
        public static async Task<IDataRecord> FirstOrDefaultAsync<TQuery, TObject>( this IDataQuery<TQuery> query, CancellationToken cancellationToken = default )
            where TQuery : IDataQuery<TQuery>, new()
        {
            ThrowIfQueryIsNull( query );
            var records = await query.GetTypedQuery()
                .TopN( 1 )
                .ToListAsync( cancellationToken );

            return records?.FirstOrDefault();
        }

        /// <summary> Asynchronously executes the given <paramref name="query"/>, returning the first result. </summary>
        /// <param name="query"> The query to execute. </param>
        /// <returns> The typed <typeparamref name="TObject"/>s. </returns>
        public static async Task<IDataRecord> FirstOrDefaultAsync<TQuery, TObject>( this IDataQuery<TQuery> query, Func<IDataRecord, bool> predicate, CancellationToken cancellationToken = default )
            where TQuery : IDataQuery<TQuery>, new()
        {
            ThrowIfQueryIsNull( query );
            var records = await query.GetTypedQuery()
                .ToListAsync( cancellationToken );

            return records?.FirstOrDefault( predicate );
        }

        /// <summary> Extends the functionality of <see cref="IDataQuery{TQuery}.WithSource(DataSet)"/> by generating a <c>SELECT FROM VALUES</c> <see cref="QuerySourceTable"/> from the given <paramref name="data"/>. </summary>
        /// <param name="query"> The query to be configured. </param>
        /// <param name="data"> The data to source <paramref name="query"/> with. </param>
        /// <param name="alias"> An (optional) SQL alias name used for the generated <c>SELECT FROM VALUES</c> exppression. </param>
        /// <returns> The configured query. </returns>
        public static TQuery From<TQuery>( this IDataQuery<TQuery> query, DataSet data, string alias = "Data" )
            where TQuery : IDataQuery<TQuery>, new()
        {
            if( data == null )
            {
                throw new ArgumentNullException( nameof( data ) );
            }

            var typedQuery = query.GetTypedQuery()
                .WithSource( data );

            if( data.Tables.Count > 0 )
            {
                var table = data.Tables[ 0 ];
                for( int i = 1; i < data.Tables.Count - 1; i++ )
                {
                    table.Merge( data.Tables[ i ] );
                }

                var columns = table.Columns.Cast<DataColumn>()
                    .Select( column => column.ToString() );

                var values = table.Rows.Cast<DataRow>()
                    .Select( row => row.ItemArray.Select( value => SqlHelper.GetSqlValue( value ) ) )
                    .Select( items => $"( {string.Join( ",", items )} )" );

                if( !values.Any() )
                {
                    values = new[]
                    {
                        $"( {string.Join( ", ", columns.Select( _ => "NULL" ) )} )"
                    };
                }

                typedQuery.DefaultQuerySource = new QuerySourceTable(
                    $"( SELECT * FROM ( VALUES {string.Join( ",", values )} ) _{alias} ( {string.Join( ",", columns )} ) {( !values.Any() ? "WHERE 1!=1" : string.Empty )} )",
                    alias
                );
            }

            return typedQuery;
        }

        /// <summary> Resets any prior OrderBy columns for the query and sets new OrderBy columns in the process. </summary>
        public static TQuery ResetOrderBy<TQuery>( this IDataQuery<TQuery> query, IEnumerable<OrderByColumn> orderByColumns )
            where TQuery : IDataQuery<TQuery>, new()
        {
            ThrowIfQueryIsNull( query );

            var typedQuery = query.GetTypedQuery();
            if( orderByColumns?.Any() != true )
            {
                typedQuery.OrderByColumns = string.Empty;
            }
            else
            {
                typedQuery.OrderByColumns = string.Join( ", ", orderByColumns.Select( column => column.ToString() ) );
            }

            return typedQuery;
        }

        /// <summary> Resets any prior OrderBy columns for the query and sets new OrderBy columns in the process. </summary>
        public static TQuery ResetOrderBy<TQuery>( this IDataQuery<TQuery> query, params OrderByColumn[] orderByColumns )
            where TQuery : IDataQuery<TQuery>, new()
            => ResetOrderBy( query, orderByColumns.AsEnumerable() );

        /// <summary> Resets any prior OrderBy columns for the query and sets new OrderBy columns in the process. </summary>
        public static TQuery ResetOrderBy<TQuery>( this IDataQuery<TQuery> query, string columnName, OrderDirection direction = OrderDirection.Default )
            where TQuery : IDataQuery<TQuery>, new()
            => ResetOrderBy( query, new OrderByColumn( columnName, direction ) );

        /// <summary> Asynchronously execute the given <paramref name="query"/>. </summary>
        /// <param name="query"> The query to execute. </param>
        public static async Task<List<IDataRecord>> ToListAsync<TQuery>( this IDataQuery<TQuery> query, CancellationToken cancellationToken = default )
            where TQuery : IDataQuery<TQuery>, new()
        {
            ThrowIfQueryIsNull( query );

            var results = await query.GetTypedQuery()
                .GetEnumerableResultAsync( cancellationToken: cancellationToken )
                .ConfigureAwait( false );

            return results?.ToList();
        }

        /// <summary> Adds a CTE to the query. </summary>
        /// <param name="query"> The query to add the CTE to. </param>
        /// <param name="cteName"> The name of the CTE. </param>
        /// <param name="cteQuery"> The inner body of the CTE. </param>
        /// <param name="cteColumns"> The columns returned by the CTE. </param>
        /// <returns> The modified query. </returns>
        public static TQuery WithCte<TQuery>( this IDataQuery<TQuery> query, string cteName, IDataQuery cteQuery, string[] cteColumns = null )
            where TQuery : IDataQuery<TQuery>, new()
        {
            ThrowIfQueryIsNull( query );
            if( string.IsNullOrWhiteSpace( cteName ) )
            {
                throw new ArgumentNullException( nameof( cteName ) );
            }

            ThrowIfQueryIsNull( cteQuery, nameof( cteQuery ) );
            string cteExpression = string.Format(
                CteTemplate,
                cteName,
                cteColumns?.Any() == true ? $"( {string.Join( ", ", cteColumns )} )" : string.Empty,
                cteQuery.GetFullQueryText( true ) // TODO: copy, instead of expanding parameters
            );

            string createCteRegion( ) => $"{CteRegionStart}\n{cteExpression}\n{CteRegionEnd}\n\n";

            // Get the typed version of the query
            TQuery typedQuery = query.GetTypedQuery();
            typedQuery.EnsureParameters();

            string queryBefore = typedQuery.Parameters.QueryBefore;
            if( string.IsNullOrWhiteSpace( queryBefore ) )
            {
                // No existing QueryBefore text, set it to the cte region's text
                queryBefore = createCteRegion();
            }
            else
            {
                // There's existing QueryBefore text, check for the cte region
                int endRegionIndex = queryBefore.LastIndexOf( CteRegionEnd );
                if( endRegionIndex > 0 )
                {
                    // The cte region exists in the QueryBefore text, so insert the cteExpression into it
                    // NOTE: SQL syntax requires ctes to be comma separated
                    queryBefore = queryBefore.Insert( endRegionIndex - 1, $",{cteExpression.Replace( CteWithKeyword, string.Empty )}" );
                }
                else
                {
                    // There's existing QueryBefore text, but it doesn't contain the cte region, so add it.
                    queryBefore += createCteRegion();
                }
            }

            // Set the new QueryBefore text on the query
            typedQuery.Parameters.QueryBefore = queryBefore;
            return typedQuery;
        }

        private static void ThrowIfQueryIsNull( IDataQuery query, string name = null )
        {
            if( query == null )
            {
                throw new ArgumentNullException( name ?? nameof( query ) );
            }
        }

    }

}
