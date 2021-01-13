using System;
using System.Collections.Generic;
using System.Reflection;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Extensions
{

    internal static class TypeExtensions
    {

        public static IEnumerable<MethodInfo> GetMethodsFrom( this Type type, Type interfaceType )
        {
            if( interfaceType == null )
            {
                throw new ArgumentNullException( nameof( interfaceType ) );
            }

            if( !interfaceType.IsInterface )
            {
                throw new ArgumentException( $"Type '{interfaceType.Name}' is not an interface.", nameof( interfaceType ) );
            }

            if( !interfaceType.IsAssignableFrom( type ) )
            {
                throw new ArgumentException( $"Interface '{interfaceType.Name}' is not implemented by '{type.Name}'.", nameof( interfaceType ) );
            }

            var interfaceMap = type.GetInterfaceMap( interfaceType );
            return interfaceMap.TargetMethods;
        }

    }

}
