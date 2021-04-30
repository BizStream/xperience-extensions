using System;
using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.DocumentEngine
{

    /// <summary> Extensions to <see cref="Type"/>. </summary>
    public static class TypeExtensions
    {
        #region Fields
        private const string ClassNameFieldName = "CLASS_NAME";
        private static readonly Type TreeNodeType = typeof( TreeNode );
        #endregion

        /// <summary> Retrieve the value of the <c>CLASS_NAME</c> constant defined on implementations of <seealso cref="TreeNode"/>. </summary>
        /// <returns> The value of the <c>CLASS_NAME</c> constant, if the current Type is an implementation of <seealso cref="TreeNode"/>. </returns>
        public static string GetNodeClassNameValue( this Type type )
        {
            if( type == null )
            {
                throw new ArgumentNullException( nameof( type ) );
            }

            return !TreeNodeType.IsAssignableFrom( type )
                ? throw new InvalidOperationException( $"Type '{type.Name}' is not a {nameof( TreeNode )}, cannot retrieve the constant '{ClassNameFieldName}' value." )
                : type.GetField( ClassNameFieldName )
                    .GetRawConstantValue() as string;
        }

    }

}