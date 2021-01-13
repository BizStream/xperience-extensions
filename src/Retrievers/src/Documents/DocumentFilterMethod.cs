using System;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> Indicates the method to use for filter documents. </summary>
    public enum DocumentFilterMethod
    {

        /// <summary> Indicates that an Inner Join should be used for filtering. </summary>
        // [Obsolete( "InnerJoins are not well supported by the Kentico API, and may cause unexpected results or errors." )]
        InnerJoin,

        /// <summary> Indicates that an Inner Query where condition should be used for filtering. </summary>
        InnerQuery,

        /// <summary> Indicates that a value should be used for filtering. </summary>
        /// <remarks> Use of this filtering method may commonly result in the execution of additional queries to retrieve values required for filter implementations. </remarks>
        Value

    }

}
