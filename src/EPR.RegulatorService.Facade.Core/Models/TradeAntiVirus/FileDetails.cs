using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus
{
    [ExcludeFromCodeCoverage]
    public class FileDetails
    {
        #pragma warning disable CA1822 // Mark members as static
        public string Service => "epr";
        #pragma warning restore CA1822 // Mark members as static

        public Guid Key { get; set; }

        public string Collection { get; set; }

        public string Extension { get; set; }

        public string FileName { get; set; }

        public Guid UserId { get; set; }

        public string UserEmail { get; set; }

        public string Content { get; set; }

        public bool PersistFile { get; set; }
    }
}
