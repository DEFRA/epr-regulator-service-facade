using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels
{
    public class OrganisationRegistrationSubmissionEmailModel : UserEmailModel
    {
        public string OrganisationNumber { get; set; }

        public string OrganisationName { get; set; }
    }
}
