using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BizStream.Kentico.Xperience.AspNetCore.Mvc.Testing;
using CMS.Base;
using CMS.CMSImportExport;
using CMS.DocumentEngine;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using NUnit.Framework;

namespace BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers.Tests.Abstractions
{

    public class PageRetrieversTests<TStartup> : AutomatedTestsWithIsolatedWebApplication<TStartup>
        where TStartup : class
    {

        #region Fields
        private const string ImportPackagePath = @"CMSSiteUtils\Import\BizStream_PageRetrievers_Tests.zip";
        #endregion

        [SetUp]
        public async Task PageRetrieversTestsSetUp( )
            => SeedData();

        protected override XperienceWebApplicationFactory<TStartup> CreateWebApplicationFactory( )
        {
            var factory = base.CreateWebApplicationFactory();
            factory.Server.PreserveExecutionContext = true;

            return factory;
        }

        private void ImportObjectsData( )
        {
            var settings = new SiteImportSettings( UserInfo.Provider.Get( "administrator" ) )
            {
                EnableSearchTasks = false,
                ImportType = ImportTypeEnum.AllNonConflicting,
                SourceFilePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, ImportPackagePath ),
                WebsitePath = SystemContext.WebApplicationPhysicalPath
            };

            settings.LoadDefaultSelection();

            ImportProvider.ImportObjectsData( settings );
            ImportProvider.DeleteTemporaryFiles( settings, false );
        }

        protected virtual void SeedData( )
        {
            ImportObjectsData();

            var site = SeedSite();
            var root = SeedRootNode( site );

            SeedTestNodes( root );
        }

        private TreeNode SeedRootNode( SiteInfo site )
        {
            var data = new DataContainer();
            data.SetValue( nameof( TreeNode.NodeSiteID ), site.SiteID );

            var root = TreeNode.New<TreeNode>( SystemDocumentTypes.Root, data );
            root.Insert( null, false );

            return root;
        }

        private SiteInfo SeedSite( )
        {
            var site = SiteInfo.Provider.Get( "NewSite" );
            CultureSiteInfo.Provider.Add(
                CultureInfo.Provider.Get( "en-US" ).CultureID,
                site.SiteID
            );

            SiteContext.CurrentSite = site;
            return site;
        }

        private void SeedTestNodes( TreeNode parent, int count = 3, int depth = 3, Action<TreeNode>? seed = null )
        {
            foreach( var order in Enumerable.Range( 0, count ) )
            {
                var node = new TestNode
                {
                    DocumentName = $"{order}",
                    Heading = $"{nameof( TestNode )} {depth}-{order}"
                };

                seed?.Invoke( node );

                node.Insert( parent );
                if( depth - 1 > 0 )
                {
                    SeedTestNodes( node, count, depth - 1, seed );
                }
            }
        }

    }

}
