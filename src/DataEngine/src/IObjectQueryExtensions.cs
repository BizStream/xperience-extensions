using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine
{

    /// <summary> Extensions to <see cref="IObjectQuery"/>. </summary>
    public static class IObjectQueryExtensions
    {

        /// <summary> Asynchronously executes the given <paramref name="query"/>, returning the first result. </summary>
        /// <param name="query"> The query to execute. </param>
        /// <typeparam name="TQuery"> The type of <see cref="IObjectQuery"/> to execute. </typeparam>
        /// <typeparam name="TObject"> The type of <see cref="BaseInfo"/> object being queried. </typeparam>
        /// <returns> The typed <typeparamref name="TObject"/>s. </returns>
        /// <remarks> This method calls <see cref="IDataQuerySettings{TQuery}.TopN(int)"/> on the given <paramref name="query"/> to ensure only a single record is returned from the DB. </remarks>
        public static Task<TObject> FirstOrDefaultAsync<TQuery, TObject>( this IObjectQuery<TQuery, TObject> query, CancellationToken cancellationToken = default )
            where TQuery : IObjectQuery<TQuery, TObject>
            where TObject : BaseInfo
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            return query.GetTypedQuery()
                .TopN( 1 )
                .ToListAsync( cancellationToken )
                .ContinueWith( task => task.Result?.FirstOrDefault(), cancellationToken );
        }

        /// <summary> Asynchronously executes the given <paramref name="query"/>, returning the first result. </summary>
        /// <param name="query"> The query to execute. </param>
        /// <typeparam name="TQuery"> The type of <see cref="IObjectQuery"/> to execute. </typeparam>
        /// <typeparam name="TObject"> The type of <see cref="BaseInfo"/> object being queried. </typeparam>
        /// <returns> The typed <typeparamref name="TObject"/>s. </returns>
        public static Task<TObject> FirstOrDefaultAsync<TQuery, TObject>( this IObjectQuery<TQuery, TObject> query, Func<TObject, bool> predicate, CancellationToken cancellationToken = default )
            where TQuery : IObjectQuery<TQuery, TObject>
            where TObject : BaseInfo
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            return query.GetTypedQuery()
                .ToListAsync( cancellationToken )
                .ContinueWith( ( task, state ) => task.Result?.FirstOrDefault( ( Func<TObject, bool> )state ), predicate, cancellationToken );
        }

        /// <summary> Asynchronously executes the given <paramref name="query"/>. </summary>
        /// <param name="query"> The query to execute. </param>
        /// <typeparam name="TQuery"> The type of <see cref="IObjectQuery"/> to execute. </typeparam>
        /// <typeparam name="TObject"> The type of <see cref="BaseInfo"/> object being queried. </typeparam>
        /// <returns> The typed <typeparamref name="TObject"/>s. </returns>
        public static async Task<List<TObject>> ToListAsync<TQuery, TObject>( this IObjectQuery<TQuery, TObject> query, CancellationToken cancellationToken = default )
            where TQuery : IObjectQuery<TQuery, TObject>
            where TObject : BaseInfo
        {
            if( query == null )
            {
                throw new ArgumentNullException( nameof( query ) );
            }

            var results = await query.GetTypedQuery()
                .GetEnumerableTypedResultAsync( cancellationToken: cancellationToken )
                .ConfigureAwait( false );

            return results?.ToList();
        }

    }

}
