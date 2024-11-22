using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels
{
    public class OrganisationRegistrationSubmissionEmailModel
    {
        public string Email { get; set; } // to email address
        public string OrganisationNumber { get; set; } // Reference

        public string OrganisationName { get; set; }

        public string Agency { get; set; }

        public string Period { get; set; }

        public string Accepted_Comment { get; set; }

        public string Cancelled_Comment { get; set; }

        public string Query_Comment { get; set; }

        public string Reject_Comment { get; set; }
        public bool? IsWelsh { get; init; } = false;

        public Dictionary<string, object> GetParameters {
            get
            {
                var parameters = new Dictionary<string, object>
                { 
                    { "email", this.Email },
                    { "organisation_number", this.OrganisationNumber },
                    { "organisation_name", this.OrganisationName },
                    { "agency", this.Agency },
                    { "year", this.Period },
                    { "accepted_comment", this.Accepted_Comment },
                    { "cancelled_comment", this.Cancelled_Comment },
                    { "query_comment", this.Query_Comment },
                    { "reject_comment", this.Reject_Comment }
                };

                return parameters; 
            } 
        }
    }
}
