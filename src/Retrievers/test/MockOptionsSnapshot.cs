using Microsoft.Extensions.Options;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Tests
{

    public class MockOptionsSnapshot<TOptions> : IOptionsSnapshot<TOptions>
        where TOptions : class, new()
    {

        public TOptions Value { get; set; }

        public MockOptionsSnapshot( TOptions options )
            => Value = options;

        public TOptions Get( string name )
            => Value;

    }

}
