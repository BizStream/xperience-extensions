using System;
using System.IO;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Abstractions.CustomTables;
using BizStream.Extensions.Kentico.Xperience.Retrievers.CustomTables;
using BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.Retrievers;
using CMS.Base;
using CMS.CMSImportExport;
using CMS.Membership;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests.CustomTables
{

    public abstract class BaseCustomTableItemRetrieverTests : BaseRetrieverIntegrationTests
    {

        protected virtual ICustomTableItemRetriever CreateCustomTableItemRetriever( )
            => new CustomTableItemRetriever();

        protected override void InitApplication( )
        {
            base.InitApplication();

            // Creates an object containing the import settings
            var settings = new SiteImportSettings( MembershipContext.AuthenticatedUser )
            {
                ImportType = ImportTypeEnum.AllForced,
                SourceFilePath = Path.Combine( AppContext.BaseDirectory, "data\\custom-table-types.zip" ),
                WebsitePath = SystemContext.WebApplicationPhysicalPath,
            };

            settings.LoadDefaultSelection();

            // Imports the package
            ImportProvider.ImportObjectsData( settings );

            // Deletes temporary data
            ImportProvider.DeleteTemporaryFiles( settings, false );

            InitCustomTableItems();
        }

        protected abstract void InitCustomTableItems( );

    }

}
