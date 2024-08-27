namespace EPR.RegulatorService.Facade.Core.Exceptions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[ExcludeFromCodeCoverage]
[Serializable]
public class RegulatorFacadeException : Exception
{
    public RegulatorFacadeException()
    {
    }

    public RegulatorFacadeException(string message)
        : base(message)
    {
    }

    public RegulatorFacadeException(string message, Exception inner)
        : base(message, inner)
    {
    }

// Disable the warning.
#pragma warning disable SYSLIB0051
    protected RegulatorFacadeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
// Re-enable the warning.
#pragma warning restore SYSLIB0051
}