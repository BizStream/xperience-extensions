using CMS.DocumentEngine;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    /// <summary> Represents options for querying sibling documents. </summary>
    public class SiblingsDocumentRetrieverOptions
    {

        /// <summary> The method to use for filtering. </summary>
        /// <remarks> If <see cref="DocumentFilterMethod.Value"/> is used, an additional query will be executed to retrieve the <see cref="TreeNode.NodeID" /> of the parent <see cref="TreeNode"/> to filter by. </remarks>
        /// <value> <see cref="DocumentFilterMethod.InnerJoin"/>. </value>
        public DocumentFilterMethod FilterMethod { get; set; } = DocumentFilterMethod.InnerJoin;

    }

}
