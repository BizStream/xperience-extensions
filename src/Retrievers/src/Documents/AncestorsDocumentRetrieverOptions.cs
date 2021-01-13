using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> Represents options for querying ancestor documents. </summary>
    public class AncestorsDocumentRetrieverOptions
    {

        /// <summary> The method to use for filtering. </summary>
        /// <remarks> If <see cref="DocumentFilterMethod.Value"/> is used, an additional query will be executed to retrieve the <see cref="TreeNode.NodeID" />s of the ancestor <see cref="TreeNode"/>s to filter. </remarks>
        /// <value> <see cref="DocumentFilterMethod.InnerJoin"/>. </value>
        public DocumentFilterMethod FilterMethod { get; set; } = DocumentFilterMethod.InnerJoin;

    }

}
