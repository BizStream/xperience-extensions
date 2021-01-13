using System;
using CMS.DataEngine;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine
{

    /// <summary> Extensions to <see cref="Type"/>. </summary>
    public static class TypeExtensions
    {
        #region Fields
        private const string ObjectTypeFieldName = "OBJECT_TYPE";
        private static readonly Type BaseInfoType = typeof( BaseInfo );
        #endregion

        /// <summary> Retrieve the value of the <c>OBJECT_TYPE</c> constant defined on implementations of <seealso cref="BaseInfo"/>. </summary>
        /// <returns> The value of the <c>OBJECT_TYPE</c> constant, if the current Type is an implementation of <seealso cref="BaseInfo"/>. </returns>
        public static string GetObjectTypeValue( this Type type )
        {
            if( type == null )
            {
                throw new ArgumentNullException( nameof( type ) );
            }

            return !BaseInfoType.IsAssignableFrom( type )
                ? throw new InvalidOperationException( $"Type '{type.Name}' is not of type '{nameof( BaseInfo )}', cannot retrieve the constant '{ObjectTypeFieldName}' value." )
                : type.GetField( ObjectTypeFieldName )
                    .GetRawConstantValue() as string;
        }

    }

}
