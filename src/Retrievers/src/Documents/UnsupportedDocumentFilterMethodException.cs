using System;

namespace BizStream.Extensions.Kentico.Xperience.Retrievers.Documents
{

    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Design", "CA1032:Implement standard exception constructors", Justification = "The lack of standard exception constructors is intentional." )]
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Usage", "CA2229:Implement serialization constructors", Justification = "Not required." )]
    public class UnsupportedDocumentFilterMethodException : NotSupportedException
    {

        public UnsupportedDocumentFilterMethodException( DocumentFilterMethod filterMethod )
            : base( $"'{nameof( DocumentFilterMethod )}.{filterMethod.ToString()}' is not supported." )
        {
        }

    }
}
