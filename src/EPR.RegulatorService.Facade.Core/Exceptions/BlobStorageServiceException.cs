namespace EPR.RegulatorService.Facade.Core.Exceptions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[ExcludeFromCodeCoverage]
[Serializable]
public class BlobStorageServiceException : Exception
{
    public BlobStorageServiceException()
    {
    }

    public BlobStorageServiceException(string message)
        : base(message)
    {
    }

    public BlobStorageServiceException(string message, Exception inner)
        : base(message, inner)
    {
    }

// Disable the warning.
#pragma warning disable SYSLIB0051
    protected BlobStorageServiceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
// Re-enable the warning.
#pragma warning restore SYSLIB0051
}