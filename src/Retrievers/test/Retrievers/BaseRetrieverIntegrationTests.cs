using System;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Extensions;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Retrievers
{

    public abstract class BaseRetrieverIntegrationTests : BaseIntegrationTests
    {
        protected abstract Type RetrieverType { get; }
        protected abstract Type RetrieverInterfaceType { get; }

        [Test]
        public virtual void Retriever_InterfaceMethods_ShouldBeOverridable( )
        {
            var interfaceMethods = RetrieverType.GetMethodsFrom( RetrieverInterfaceType );
            foreach( var interfaceMethod in interfaceMethods )
            {
                Assert.IsTrue( interfaceMethod.IsVirtual, $"Implementation of method '{interfaceMethod.DeclaringType.Name}.{interfaceMethod.Name}' is not marked as virtual." );
            }
        }

    }

}
