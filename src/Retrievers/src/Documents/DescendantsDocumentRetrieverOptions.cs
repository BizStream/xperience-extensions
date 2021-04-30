using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> Represents options for querying descendant documents. </summary>
    public class DescendantsDocumentRetrieverOptions
    {

        /// <summary> The method to use for filtering. </summary>
        /// <remarks> If <see cref="DocumentFilterMethod.Value"/> is used, an additional query will be executed to retrieve the <see cref="TreeNode.NodeID" />s of the descendant <see cref="TreeNode"/>s to filter. Usage of <see cref="DocumentFilterMethod.InnerJoin"/> may result in under-performant <see cref="MultiDocumentQuery"/>s.
        /// </remarks>
        /// <value> <see cref="DocumentFilterMethod.InnerJoin"/>. </value>
        public DocumentFilterMethod FilterMethod { get; set; } = DocumentFilterMethod.InnerJoin;

    }

}
