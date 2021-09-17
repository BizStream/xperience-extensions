using System;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine
{

    /// <summary> Extensions to <see cref="QuerySourceBase{TSource}"/>. </summary>
    public static class QuerySourceBaseExtensions
    {

        /// <summary> Joins the given query. </summary>
        /// <typeparam name="TQuerySource"> The type of query to extend. </typeparam>
        /// <typeparam name="TDataQuery"> The type of query being joined. </typeparam>
        /// <param name="source"> The query source (left side). </param>
        /// <param name="query"> The query to join (right side). </param>
        /// <param name="leftColumn"> The column of the left side to join on. </param>
        /// <param name="rightColumn"> The column on the right side to join on. </param>
        /// <param name="condition"> An additional condition to apply to the join. </param>
        /// <param name="joinType"> The type of join to perform. </param>
        /// <returns> The modified <paramref name="source"/>. </returns>
        /// <exception cref="ArgumentException" />
        public static TQuerySource Join<TQuerySource, TDataQuery>( this TQuerySource source, TDataQuery query, string leftColumn, string rightColumn, IWhereCondition? condition = null, JoinTypeEnum joinType = JoinTypeEnum.Inner )
            where TQuerySource : QuerySourceBase<TQuerySource>, new()
            where TDataQuery : IDataQuery
            => source.Join( query, null, leftColumn, rightColumn, condition, joinType );

        /// <summary> Joins the given query with an alias. </summary>
        /// <typeparam name="TQuerySource"> The type of query to extend. </typeparam>
        /// <typeparam name="TDataQuery"> The type of query being joined. </typeparam>
        /// <param name="source"> The query source (left side). </param>
        /// <param name="query"> The query to join (right side). </param>
        /// <param name="alias"> The alias to use for the right side of the join. </param>
        /// <param name="leftColumn"> The column of the left side to join on. </param>
        /// <param name="rightColumn"> The column on the right side to join on. </param>
        /// <param name="condition"> An additional condition to apply to the join. </param>
        /// <param name="joinType"> The type of join to perform. </param>
        /// <returns> The modified <paramref name="source"/>. </returns>
        /// /// <exception cref="ArgumentException" />
        public static TQuerySource Join<TQuerySource, TDataQuery>( this TQuerySource source, TDataQuery query, string? alias, string leftColumn, string rightColumn, IWhereCondition? condition = null, JoinTypeEnum joinType = JoinTypeEnum.Inner )
            where TQuerySource : QuerySourceBase<TQuerySource>, new()
            where TDataQuery : IDataQuery
        {
            if( source is null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }

            if( query is null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            query.ApplyParametersTo( source );
            return source.Join( query.ToQuerySourceTable( alias, false ), leftColumn, rightColumn, condition, joinType );
        }

    }
}
