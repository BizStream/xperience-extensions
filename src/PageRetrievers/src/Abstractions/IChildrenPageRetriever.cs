using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CMS.DocumentEngine;
using Kentico.Content.Web.Mvc;

namespace BizStream.Extensions.Kentico.Xperience.PageRetrievers.Abstractions
{

    public interface IChildrenPageRetriever
    {

        Task<IEnumerable<TNode>> RetrieveAsync<TNode>(
            int nodeID,
            Action<DocumentQuery<TNode>>? queryFilter = null,
            Action<IPageCacheBuilder<TNode>>? configureCache = null,
            CancellationToken cancellation = default
        )
            where TNode : TreeNode, new();

        Task<IEnumerable<TreeNode>> RetrieveMultipleAsync(
            int nodeID,
            Action<MultiDocumentQuery>? queryFilter = null,
            Action<IPageCacheBuilder<TreeNode>>? configureCache = null,
            CancellationToken cancellation = default
        );

    }

}
