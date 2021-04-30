# Xperience Caching Extensions

The package provides extension methods and utilities to facilitate caching in a .NET+Xperience solution.

Features:

- Fluent API for configuring the `CacheKeys` of a `CMSCacheDependency`
- Support for expiring an `Microsoft.Extensions.Caching.Memory.ICacheEntry` on a configured `CMSCacheDependency`

## Usage

Install the package into an Xperience project using `dotnet`:

```bash
dotnet add package BizStream.Extensions.Kentico.Xperience.Caching
```

OR, by adding a `PackageReference`:

```csproj
<PackageReference Include="BizStream.Extensions.Kentico.Xperience.Caching" Version="x.x.x" />
```

### Examples

#### Fluent CacheKeys:

```csharp
new CMSCacheDependency()
    .OnObject<UserInfo>( "administrator" )
    .OnObjectsOfType<UserRoleInfo>()
    .OnNode( node );
```

#### `ICacheEntry` Dependency Support:

```csharp
public class ArticleService
{
    private readonly IMemoryCache cache;

    public ArticleService( IMemoryCache cache )
        => this.cache = cache;

    public ArticleNode GetArticle( Guid nodeGuid )
        => cache.GetOrCreate(
            $"article|node|{nodeGuid}",
            entry =>
            {
                var node = DocumentHelper.GetDocuments<ArticleNode>()
                    .WhereEquals( nameof( ArticleNode.NodeGUID ), nodeGuid )
                    .TopN( 1 )
                    .FirstOrDefault();

                if( node == null )
                {
                    entry.SetAbsoluteExpiration( TimeSpan.FromMinutes( 20 ) );
                    return null;
                }

                // configure the ICacheEntry with a dependency on the node
                entry.SetCMSDependency( $"nodeid|{node.NodeID}" );

                // OR, use the fluent CacheKeys api
                entry.WithCMSDependency( depends => depends.OnNode( node.NodeID ) );

                return node;
            }
        );

}

```
