using System;
using System.IO;
using System.Linq;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.Documents;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Documents;
using CMS.Base;
using CMS.CMSImportExport;
using CMS.DocumentEngine;
using CMS.Membership;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Retrievers.Documents
{

    public abstract class BaseDocumentRetrieverIntegrationTests : BaseRetrieverIntegrationTests
    {
        #region Fields
        protected TreeNode RootNode;
        #endregion

        protected virtual IDocumentRetriever CreateDocumentRetriever( DocumentRetrieverOptions options = null )
            => new DocumentRetriever( new MockOptionsSnapshot<DocumentRetrieverOptions>( options ?? new DocumentRetrieverOptions() ) );

        protected override void InitApplication( )
        {
            base.InitApplication();

            // Creates an object containing the import settings
            var settings = new SiteImportSettings( MembershipContext.AuthenticatedUser )
            {
                ImportType = ImportTypeEnum.AllForced,
                SourceFilePath = Path.Combine( AppContext.BaseDirectory, "data\\page-types.zip" ),
                WebsitePath = SystemContext.WebApplicationPhysicalPath,
            };

            settings.LoadDefaultSelection();

            // Imports the package
            ImportProvider.ImportObjectsData( settings );

            // Deletes temporary data
            ImportProvider.DeleteTemporaryFiles( settings, false );

            RootNode = GetRootNode();
            InitContentTree();
        }

        protected abstract void InitContentTree( );

        private TreeNode GetRootNode( )
            => DocumentHelper.GetDocuments()
                .Type( SystemDocumentTypes.Root )
                .TopN( 1 )
                .First();

    }

}
