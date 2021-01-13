using System;
using System.Collections.Generic;
using BizStream.Extensions.Kentico.Xperience.DocumentEngine;
using CMS.DocumentEngine;
using CMS.Helpers;

namespace BizStream.Extensions.Kentico.Xperience.Caching
{

    /// <summary> Extensions to <see cref="CMSCacheDependency"/> to assist with fluently configuring dependency keys for <see cref="TreeNode"/>s. </summary>
    public static partial class CMSCacheDependencyExtensions
    {

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TreeNode"/> identified by the given <paramref name="documentID"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="documentID"> The <see cref="TreeNode.DocumentID"/> that identifies the <see cref="TreeNode"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnDocument( this CMSCacheDependency dependency, int documentID )
            => EnsureCacheKeys( dependency, $"documentid|{documentID}" );

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TreeNode"/> identified by the given <paramref name="nodeID"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="nodeID"> The <see cref="TreeNode.NodeID"/> that identifies the <see cref="TreeNode"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnNode( this CMSCacheDependency dependency, int nodeID )
            => EnsureCacheKeys( dependency, $"nodeid|{nodeID}" );

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TreeNode"/> identified by the given <paramref name="aliasPath"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="siteName"> The <see cref="CMS.SiteProvider.SiteInfo.SiteName"/> of the site the <see cref="TreeNode"/> resides on. </param>
        /// <param name="aliasPath"> The <see cref="TreeNode.NodeAliasPath"/> that identifies the <see cref="TreeNode"/> to configure the dependency with. </param>
        /// <param name="cultureCode"> The (optional) <see cref="TreeNode.DocumentCulture"/> that identifies the culture variant of the <see cref="TreeNode"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnNode( this CMSCacheDependency dependency, string siteName, string aliasPath, string cultureCode = null )
        {
            ThrowIfDependencyIsNull( dependency );
            ThrowIsSiteNameIsEmpty( siteName );
            if( string.IsNullOrWhiteSpace( aliasPath ) )
            {
                throw new ArgumentNullException( nameof( aliasPath ) );
            }

            var keysToAdd = new List<string>()
            {
                 $"node|{siteName}|{aliasPath}"
            };

            if( !string.IsNullOrWhiteSpace( cultureCode ) )
            {
                keysToAdd.Add( $"node|{siteName}|{aliasPath}|{cultureCode}" );
            }

            return EnsureCacheKeys( dependency, keysToAdd.ToArray() );
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on a <see cref="TreeNode"/> identified by the given <paramref name="nodeGuid"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="siteName"> The <see cref="CMS.SiteProvider.SiteInfo.SiteName"/> of the site the <see cref="TreeNode"/> resides on. </param>
        /// <param name="nodeGuid"> The <see cref="TreeNode.NodeGUID"/> that identifies the <see cref="TreeNode"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnNode( this CMSCacheDependency dependency, string siteName, Guid nodeGuid )
        {
            ThrowIfDependencyIsNull( dependency );
            ThrowIsSiteNameIsEmpty( siteName );
            if( nodeGuid == Guid.Empty )
            {
                throw new ArgumentNullException( nameof( nodeGuid ) );
            }

            return EnsureCacheKeys( dependency, $"nodeguid|{siteName}|{nodeGuid}" );
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on the given <see cref="TreeNode"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="node"> The <see cref="TreeNode"/> to configure the dependency on. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        /// <seealso cref="OnDocument(CMSCacheDependency, int)"/>
        /// <seealso cref="OnNode(CMSCacheDependency, int)"/>
        /// <seealso cref="OnNode(CMSCacheDependency, string, Guid)"/>
        /// <seealso cref="OnNode(CMSCacheDependency, string, string, string)"/>
        public static CMSCacheDependency OnNode( this CMSCacheDependency dependency, TreeNode node )
        {
            ThrowIfDependencyIsNull( dependency );
            if( node == null )
            {
                throw new ArgumentNullException( nameof( node ) );
            }

            OnDocument( dependency, node.DocumentID );
            OnNode( dependency, node.NodeID );
            OnNode( dependency, node.NodeSiteName, node.NodeGUID );
            OnNode( dependency, node.NodeSiteName, node.NodeAliasPath );

            return dependency;
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on the descendants of the <see cref="TreeNode"/> identified by the given <paramref name="aliasPath"/>. </summary>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="siteName"> The <see cref="CMS.SiteProvider.SiteInfo.SiteName"/> of the site the <see cref="TreeNode"/> resides on. </param>
        /// <param name="aliasPath"> The <see cref="TreeNode.NodeAliasPath"/> that identifies the <see cref="TreeNode"/> to configure the dependency with. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnNodeDescendants( this CMSCacheDependency dependency, string siteName, string aliasPath )
        {
            ThrowIfDependencyIsNull( dependency );
            ThrowIsSiteNameIsEmpty( siteName );
            if( string.IsNullOrWhiteSpace( aliasPath ) )
            {
                throw new ArgumentNullException( nameof( aliasPath ) );
            }

            return EnsureCacheKeys( dependency, $"node|{siteName}|{aliasPath}|childnodes" );
        }

        /// <summary> Configure the <see cref="CMSCacheDependency"/> with a dependency on any (all) <see cref="TreeNode"/>s, of type <typeparamref name="TNode"/>. </summary>
        /// <typeparam name="TNode"> The type of <see cref="TreeNode"/> to configure the dependency with. </typeparam>
        /// <param name="dependency"> The <see cref="CMSCacheDependency"/> to be configured. </param>
        /// <param name="siteName"> The <see cref="CMS.SiteProvider.SiteInfo.SiteName"/> of the site the <see cref="TreeNode"/>s to configure the dependency with reside on. </param>
        /// <returns> The configured <see cref="CMSCacheDependency"/>. </returns>
        public static CMSCacheDependency OnNodesOfType<TNode>( this CMSCacheDependency dependency, string siteName )
            where TNode : TreeNode, new()
        {
            ThrowIfDependencyIsNull( dependency );
            ThrowIsSiteNameIsEmpty( siteName );

            var className = typeof( TNode ).GetNodeClassNameValue();
            return EnsureCacheKeys( dependency, $"nodes|{siteName}|{className}|all" );
        }

    }

}
