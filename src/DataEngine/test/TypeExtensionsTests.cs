using System;
using BizStream.Extensions.Kentico.Xperience.DataEngine.Tests.Models;
using CMS.Membership;
using CMS.Taxonomy;
using CMS.Tests;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.DataEngine.Tests
{

    [TestFixture( Category = "Unit" )]
    [TestOf( typeof( TypeExtensions ) )]
    public class TypeExtensionsTests : UnitTests
    {

        [Test]
        public void GetObjectTypeValue_ShouldNotSupportNonBaseInfoTypes( [Values( typeof( int ), typeof( string ), typeof( Poco ) )] Type type )
            => Assert.Throws<InvalidOperationException>(
                ( ) => type.GetObjectTypeValue()
            );

        [Test]
        public void GetObjectTypeValue_ShouldReturnObjectType( [Values( typeof( UserInfo ), typeof( TagInfo ) )] Type type )
        {
            var objectType = type.GetObjectTypeValue();
            if( type == typeof( UserInfo ) )
            {
                Assert.AreEqual( UserInfo.OBJECT_TYPE, objectType );
            }

            if( type == typeof( TagInfo ) )
            {
                Assert.AreEqual( TagInfo.OBJECT_TYPE, objectType );
            }
        }

    }

}
