using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels
{
    public class OrganisationRegistrationSubmissionEmailModel
    {
        public string ToEmail { get; set; } // to email address
        public string ApplicationNumber { get; set; }  

        public string OrganisationNumber { get; set; } // Reference

        public string OrganisationName { get; set; }

        public string Agency { get; set; }

        public string AgencyEmail { get; set; }

        public string Period { get; set; }

        public bool? IsWelsh { get; init; } = false;

        public Dictionary<string, object> GetParameters {
            get
            {
                var parameters = new Dictionary<string, object>
                { 
                    { "to_email", this.ToEmail },
                    { "application_number", this.ApplicationNumber },
                    { "organisation_number", this.OrganisationNumber },
                    { "organisation_name", this.OrganisationName },
                    { "agency", this.Agency },
                    { "agency_email", this.AgencyEmail },
                    { "year", this.Period }
                };

                return parameters; 
            } 
        }
    }
}
