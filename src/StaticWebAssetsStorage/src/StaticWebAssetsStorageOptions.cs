using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace BizStream.Extensions.Kentico.Xperience.StaticWebAssetsStorage
{

    /// <summary> Represents options that configure the behavior of <see cref="StaticWebAssetsStorageProvider"/>s. </summary>
    public class StaticWebAssetsStorageOptions
    {

        /// <summary> The names of <see cref="Environments"/> in which to register <see cref="StaticWebAssetsStorageProvider"/>s. </summary>
        /// <value> <see cref="Environments.Development"/>. </value>
        public IList<string> EnvironmentNames { get; } = new List<string> { Environments.Development };

    }

}
