using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.DocumentEngine
{

    /// <summary> String Constants. </summary>
    public static class Strings
    {

        /// <summary> The Sql alias for the select expression when querying documents using <see cref="DocumentQuery{TDocument}"/>. </summary>
        /// <see cref="IDocumentQueryExtensions.GetQueryAliasName(IDocumentQuery)"/>
        public const string DocumentQueryAliasName = "V";

        /// <summary> The Sql alias for the select expression when querying documents using <see cref="MultiDocumentQuery"/>. </summary>
        /// <see cref="IDocumentQueryExtensions.GetQueryAliasName(IDocumentQuery)"/>
        public const string MultiDocumentQueryAliasName = "SubData";

    }

}
