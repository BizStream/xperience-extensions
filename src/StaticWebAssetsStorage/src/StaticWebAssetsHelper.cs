using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BizStream.Extensions.Kentico.Xperience.StaticWebAssetsStorage
{

    /// <summary> Static helper class for retrieving values from the RCL Bundle paths from the default manifest. </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class"> Reusable UI with Razor Class Libraries. </seealso>
    public static class StaticWebAssetsHelper
    {

        /// <summary> Retrieve <c>(BasePath, Path)</c> pairs from the default RCL Bundle Manifest. </summary>
        /// <param name="environment"> The environment to resolve the manifest within. </param>
        /// <param name="configuration"> If specified, the configuration object used to retrieve the manifest <see cref="WebHostDefaults.StaticWebAssetsKey">default location</see>. </param>
        public static IEnumerable<(string, string)> GetRCLPaths( IWebHostEnvironment environment, IConfiguration configuration = null )
        {
            if( environment == null )
            {
                throw new ArgumentNullException( nameof( environment ) );
            }

            if( configuration == null )
            {
                throw new ArgumentNullException( nameof( configuration ) );
            }

            using var source = ResolveManifest( environment, configuration );
            if( source != null )
            {
                var manifest = XDocument.Load( source );
                foreach( var element in manifest.Root.Elements() )
                {
                    var basePath = element.Attribute( "BasePath" )?.Value;
                    var path = element.Attribute( "Path" )?.Value;

                    yield return (basePath, path);
                }
            }
        }

        /*
         * The following methods were copied (and slightly modified) from the StaticWebAssets source code:
         *  https://github.com/dotnet/aspnetcore/blob/3e9ae8e5eee2930da0096ab4ca4976f5938df648/src/Hosting/Hosting/src/StaticWebAssets/StaticWebAssetsLoader.cs#L55
         *
         *  MS decided to make them `internal`, but we need them for resolving the absolute paths to configure PageBuilder to locate _our_ RCL's static files
         *  This is all required due to Kentico's `BuilderAssetsProvider` (also `internal`), being hardcoded to only use the `IWebHostEnvironment.IFileProvder`
         *  for Kentico's RCL package (`BuilderAssetsProvider.GetLibraryBundleVirtualPaths`), and `CMS.IO.FileInfo` being used for the configurable
         *  page builder assets (`BuilderAssetsProvider.GetWebRootBundleVirtualPaths`).
         */
        private static Stream ResolveManifest( IWebHostEnvironment environment, IConfiguration configuration = null )
        {
            if( environment == null )
            {
                throw new ArgumentNullException( nameof( environment ) );
            }

            if( configuration == null )
            {
                throw new ArgumentNullException( nameof( configuration ) );
            }

            try
            {
                var manifestPath = configuration?.GetValue<string>( WebHostDefaults.StaticWebAssetsKey );
                var filePath = string.IsNullOrEmpty( manifestPath )
                    ? ResolveRelativeToAssembly( environment )
                    : manifestPath;

                if( !string.IsNullOrEmpty( filePath ) && File.Exists( filePath ) )
                {
                    return File.OpenRead( filePath );
                }
                else
                {
                    // A missing manifest might simply mean that the feature is not enabled, so we simply
                    // return early. Misconfigurations will be uncommon given that the entire process is automated
                    // at build time.
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string ResolveRelativeToAssembly( IWebHostEnvironment environment )
        {
            var assembly = System.Reflection.Assembly.Load( environment.ApplicationName );
            if( string.IsNullOrEmpty( assembly.Location ) )
            {
                return null;
            }

            return Path.Combine( Path.GetDirectoryName( assembly.Location ), $"{environment.ApplicationName}.StaticWebAssets.xml" );
        }

    }

}
