using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegistrationStatus
{
    pending,
    granted,
    refused,
    queried,
    cancelled
}
