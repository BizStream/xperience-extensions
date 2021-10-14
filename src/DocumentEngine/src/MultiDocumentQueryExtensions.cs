using System;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.DataEngine;
using CMS.DataEngine;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.DocumentEngine
{

    /// <summary> Extensions to <see cref="MultiDocumentQuery"/>. </summary>
    public static class MultiDocumentQueryExtensions
    {

        /// <summary> Filters the query to Documents with a page type of <typeparamref name="TNode"/>, or whose PageType inherits from the PageType, <typeparamref name="TNode"/>. </summary>
        /// <typeparam name="TNode"> The PageType to filter inheritance by. </typeparam>
        /// <param name="query"> The query to filter. </param>
        /// <remarks>
        /// This extension uses a recursive CTE in order to provide filtering that behaves similarly to the <c>is</c> operator in C#.
        /// For Example, given PageTypes with the following inheritance:
        /// <code>
        /// A
        /// B -> A
        /// C -> B -> A
        /// D
        /// E -> D
        /// F -> B -> A
        /// </code>
        /// Calling <c>query.IsType{A}()</c> would filter the query to any document that is of type <c>A</c>, <c>B</c>, <c>C</c> or <c>F</c>.
        /// </remarks>
        // TODO: Make this extension method a typed retriever in BizStream.Extensions.Kentico.Xperience.Retrievers
        public static MultiDocumentQuery IsType<TNode>( this MultiDocumentQuery query )
            where TNode : TreeNode, new()
        {
            ThrowIfQueryIsNull( query );

            var cteName = "_PageTypes";
            var cteColumns = new[] { nameof( DataClassInfo.ClassName ), nameof( DataClassInfo.ClassID ) };

            // Query for the ClassID of the class with `className`
            var classQuery = new DataQuery()
                .From( new QuerySourceTable( new ObjectSource<DataClassInfo>() ) )
                .Columns( cteColumns )
                .WhereTrue( nameof( DataClassInfo.ClassIsDocumentType ) )
                .WhereEquals( nameof( DataClassInfo.ClassName ), typeof( TNode ).GetNodeClassNameValue() )
                .TopN( 1 );

            // Recursive CTE body; Returns ClassNames of PageTypes that inherit from the given ClassID
            var pageTypes = classQuery.UnionAll(
                new DataQuery()
                    .From( new QuerySourceTable( new ObjectSource<DataClassInfo>(), "C" ) )
                    .Columns( cteColumns.Select( c => $"C.{c}" ) )
                    .WhereTrue( nameof( DataClassInfo.ClassIsDocumentType ) )

                    // NOTE: inner join against CTE (makes the cte recursive)
                    .Source( s => s.InnerJoin( new QuerySourceTable( cteName, "pt" ), $"C.{nameof( DataClassInfo.ClassInheritsFromClassID )}", $"pt.{nameof( DataClassInfo.ClassID )}" ) )
            );

            // Query that selects from the CTE
            var classNamesQuery = new DataQuery()
                .From( cteName )
                .Column( $"DISTINCT {nameof( DataClassInfo.ClassName )}" )
                .AsSingleColumn();

            return query.WithCte( cteName, pageTypes, new[] { nameof( DataClassInfo.ClassName ), nameof( DataClassInfo.ClassID ) } )
                .WhereIn( nameof( DataClassInfo.ClassName ), classNamesQuery );
        }

        /// <summary> Includes the given type in the query, with the specified parameters. Short-hand for <see cref="MultiQueryBase{TQuery, TInnerQuery}.Type(string, Action{TInnerQuery})"/>. </summary>
        /// <typeparam name="TNode"> The type of <see cref="TreeNode"/> to be included in the query. </typeparam>
        /// <param name="query"> The query to modify. </param>
        /// <param name="parameters"> An <see cref="Action"/> that configures the inner <see cref="DocumentQuery"/> to retrieve nodes of type, <typeparamref name="TNode"/>. </param>
        public static MultiDocumentQuery Type<TNode>( this MultiDocumentQuery query, Action<DocumentQuery>? parameters = null )
            where TNode : TreeNode, new()
        {
            ThrowIfQueryIsNull( query );
            return query.Type( typeof( TNode ).GetNodeClassNameValue(), parameters );
        }

        private static void ThrowIfQueryIsNull( MultiDocumentQuery query, string? name = null )
        {
            if( query == null )
            {
                throw new ArgumentNullException( name ?? nameof( query ) );
            }
        }

    }

}
