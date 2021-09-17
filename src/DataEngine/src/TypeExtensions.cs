using System;
using CMS.CustomTables;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine
{

    /// <summary> Extensions to <see cref="Type"/>. </summary>
    public static class TypeExtensions
    {
        #region Fields
        private const string ClassNameFieldName = "CLASS_NAME";
        private const string ObjectTypeFieldName = "OBJECT_TYPE";

        private static readonly Type BaseInfoType = typeof( BaseInfo );
        private static readonly Type CustomTableItemType = typeof( CustomTableItem );
        #endregion

        /// <summary> Retrieve the value of the <c>OBJECT_TYPE</c> constant defined on implementations of <seealso cref="BaseInfo"/>. </summary>
        /// <returns> The value of the <c>OBJECT_TYPE</c> constant, if the current Type is an implementation of <seealso cref="BaseInfo"/>. </returns>
        public static string? GetObjectTypeValue( this Type type )
        {
            if( type is null )
            {
                throw new ArgumentNullException( nameof( type ) );
            }

            if( !BaseInfoType.IsAssignableFrom( type ) )
            {
                throw new InvalidOperationException( $"Type '{type.Name}' is not of type '{nameof( BaseInfo )}', cannot retrieve the constant '{ObjectTypeFieldName}' value." );
            }

            return type.GetField( ObjectTypeFieldName )
                .GetRawConstantValue() as string;
        }

        /// <summary> Retrieve the value of the <c>CLASS_NAME</c> constant defined on implementations of <seealso cref="CustomTableItem"/>. </summary>
        /// <returns> The value of the <c>CLASS_NAME</c> constant, if the current Type is an implementation of <seealso cref="CustomTableItem"/>. </returns>
        public static string? GetCustomTableClassNameValue( this Type type )
        {
            if( type is null )
            {
                throw new ArgumentNullException( nameof( type ) );
            }

            return !CustomTableItemType.IsAssignableFrom( type )
                ? throw new InvalidOperationException( $"Type '{type.Name}' is not a {CustomTableItemType.Name}, cannot retrieve the constant '{ClassNameFieldName}' value." )
                : type.GetField( ClassNameFieldName )
                    .GetRawConstantValue() as string;
        }

    }

}
