using System.Net;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.OrganisationsSearch
{
    [TestClass]
    public class OrganisationsSearchControllerTests : Controller
    {
        private readonly NullLogger<OrganisationsSearchController> _nullLogger = new();
        private readonly Mock<IProducerService> _mockProducerService = new();
        private readonly Mock<IRegulatorOrganisationService> _mockRegulatorOrganisationService = new();
        private readonly Mock<IMessagingService> _mockMessagingService = new();
        private OrganisationsSearchController _sut;

        private readonly Guid _organisationExternalId = Guid.NewGuid();
        private readonly int _currentPage = 1;
        private const int PageSize = 10;
        private const string OrganisationName = "Org";
        private MessagingConfig _messagingConfig;

        [TestInitialize]
        public void Setup()
        {
            _messagingConfig = new MessagingConfig() { 
                ApiKey = "test", 
                RemovedApprovedUserTemplateId = Guid.NewGuid().ToString(),
                InviteNewApprovedPersonTemplateId = Guid.NewGuid().ToString(),
                DemotedDelegatedUserTemplateId = Guid.NewGuid().ToString(),
                AccountCreationUrl = "FakeAccountCreationUrl"
            };
            
            var mockMessagingConfig = new Mock<IOptions<MessagingConfig>>();
            mockMessagingConfig.Setup(ap => ap.Value).Returns(_messagingConfig);
            
            _sut = new OrganisationsSearchController(_nullLogger, 
                                                     _mockProducerService.Object, 
                                                     _mockRegulatorOrganisationService.Object,
                                                     mockMessagingConfig.Object,
                                                     _mockMessagingService.Object);
            _sut.AddDefaultContextWithOid(_organisationExternalId, "TestAuth");
        }

        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_Request_Is_Valid_Then_Return_200_Success()
        {
            // Arrange
            var organisations = new OrganisationSearchResult();
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(organisations))
            });

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_Request_Is_Invalid_Then_Return_400_Bad_Request()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }
        
        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_Request_Is_Not_Successful_Then_Return_InternalServerError()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Conflict
            });

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
        
        [TestMethod]
        public async Task On_Getting_Organisation_Details_Valid_Result_Can_Be_Parsed()
        {
            // Arrange
            var companyObject = new OrganisationDetailResults();
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(companyObject))
            });

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task On_Getting_Organisation_Details_Http_Error_Result_Is_Handled()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }
        
        [TestMethod]
        public async Task On_Getting_Organisation_Details_Http_Unsuccessful_Result_Is_Handled()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
        
        [TestMethod]
        public async Task When_GetProducerOrganisationsUsersByExternalId_Is_Called_And_Request_Is_Valid_Then_Return_200_Success()
        {
            // Arrange
            var users = new List<OrganisationUserOverviewResponseModel>();
            _mockRegulatorOrganisationService.Setup(x =>
                x.GetUsersByOrganisationExternalId(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(users))
            });

            // Act
            var result = await _sut.GetUsersByOrganisationExternalId(_organisationExternalId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task When_GetProducerOrganisationsUsersByExternalId_Is_Called_And_Service_Is_InValid_Then_Null()
        {
            // Arrange
            var users = new List<OrganisationUserOverviewResponseModel>();
            _mockRegulatorOrganisationService.Setup(x =>
                x.GetUsersByOrganisationExternalId(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(users)),
                StatusCode = HttpStatusCode.BadRequest
            });

            // Act
            var result = await _sut.GetUsersByOrganisationExternalId(_organisationExternalId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task When_RemoveApprovedPerson_RemoveWithoutNomination_Valid_Result_Is_Successful()
        {
            // Arrange
            var connExternalId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            
            var associatedPerson = new List<AssociatedPersonResults>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "user",
                    Email = "test@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "RemovedApprovedUser"
                }
            };

            var request = new RemoveApprovedUsersRequest
            {
                RemovedConnectionExternalId = connExternalId,
                OrganisationId = organisationId,
                UserId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.Empty
            };

            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(request)
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(associatedPerson))
            });
            
            
            // Act
            var result = await _sut.RemoveApprovedPerson(request);
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_RemoveApprovedPerson_Http_Error_Result_Is_Handled()
        {
            // Arrange
            
            var request = new RemoveApprovedUsersRequest
            {
                OrganisationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RemovedConnectionExternalId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.Empty
            };
            
            _mockProducerService
                .Setup(c => c.RemoveApprovedUser(request))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });
            
            
            // Act
            var result = await _sut.RemoveApprovedPerson(request); 
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task When_RemoveApprovedPerson_Http_Unsuccessful_Result_Is_Handled()
        {
            // Arrange
            var request = new RemoveApprovedUsersRequest
            {
                OrganisationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RemovedConnectionExternalId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.Empty
            };
            
            _mockProducerService
                .Setup(c => c.RemoveApprovedUser(request))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            
            // Act
            var result = await _sut.RemoveApprovedPerson(request);
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
        
        [TestMethod]
        public async Task When_RemoveApprovedPerson_NominateWithoutRemoving_Valid_Result_Is_Successful()
        {
            // Arrange
            var associatedPerson = new List<AssociatedPersonResults>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "user",
                    Email = "test@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "PromotedApprovedUser"
                }
            };

            var request = new RemoveApprovedUsersRequest
            {
                RemovedConnectionExternalId =  Guid.Empty,
                OrganisationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.NewGuid()
            };

            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(request)
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(associatedPerson))
            });
            
            // Act
            var result = await _sut.RemoveApprovedPerson(request);
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_RemoveApprovedPerson_RemoveWithNominate_Valid_Result_Is_Successful()
        {
            // Arrange
            var associatedPerson = new List<AssociatedPersonResults>
            {
                new()
                {
                    FirstName = "remove",
                    LastName = "user",
                    Email = "remove@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "RemovedApprovedUser"
                },
                new()
                {
                    FirstName = "nominate",
                    LastName = "user",
                    Email = "nominate@user.com",
                    OrganisationId = "85214",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "PromotedApprovedUser"
                }
            };

            var request = new RemoveApprovedUsersRequest
            {
                RemovedConnectionExternalId =  Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.NewGuid()
            };

            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(request)
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(associatedPerson))
            });
            
            // Act
            var result = await _sut.RemoveApprovedPerson(request);
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task ValidRequest_RemoveApprovedUser_RemoveWithoutNomination_ShouldSendEmailToSendRemovedApprovedPersonNotification()
        {
           // Arrange
           var associatedPerson = new List<AssociatedPersonResults>
           {
               new()
               {
                   FirstName = "test",
                   LastName = "user",
                   Email = "test@user.com",
                   OrganisationId = "12545",
                   CompanyName = "Test Company",
                   ServiceRoleId = 1,
                   EmailNotificationType = "RemovedApprovedUser"
               }
           };

           var request = new RemoveApprovedUsersRequest
           {
               RemovedConnectionExternalId = Guid.NewGuid(),
               OrganisationId = Guid.NewGuid(),
               UserId = Guid.NewGuid(),
               PromotedPersonExternalId = Guid.Empty
           };

           _mockProducerService.Setup(x =>
               x.RemoveApprovedUser(request)
           ).ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonConvert.SerializeObject(associatedPerson))
           });

           _mockMessagingService.Setup(x =>
               x.SendRemovedApprovedPersonNotification(It.IsAny<AssociatedPersonResults>(), It.IsAny<string>()));
            
            
           // Act
           
           _ = await _sut.RemoveApprovedPerson(request);
            
            // Assert
           
            _mockMessagingService
                .Verify(x => x.SendRemovedApprovedPersonNotification(
                        It.Is<AssociatedPersonResults>(result => 
                            result.TemplateId == _messagingConfig.RemovedApprovedUserTemplateId), 
                        It.IsAny<string>()),
                    Times.Exactly(1));
        }
        
        [TestMethod]
        public async Task ValidRequest_RemoveApprovedUser_RemoveWithNomination_ShouldSendEmailToSendRemovedApprovedPersonNotification()
        {
            // Arrange
            var associatedPerson = new List<AssociatedPersonResults>
            {
                new()
                {
                    FirstName = "removed",
                    LastName = "user",
                    Email = "removed@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "RemovedApprovedUser"
                },
                new()
                {
                    FirstName = "nominated",
                    LastName = "user",
                    Email = "nominated@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "PromotedApprovedUser"
                },
                new()
                {
                    FirstName = "",
                    LastName = "",
                    Email = "nominated@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "PromotedApprovedUser"
                }
            };

            var request = new RemoveApprovedUsersRequest
            {
                RemovedConnectionExternalId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.NewGuid()
            };

            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(request)
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(associatedPerson))
            });

            _mockMessagingService.Setup(x =>
                x.SendRemovedApprovedPersonNotification(It.IsAny<AssociatedPersonResults>(), It.IsAny<string>()));
            
            // Act
           _ = await _sut.RemoveApprovedPerson(request);
            
            // Assert
           
            _mockMessagingService
                .Verify(x => x.SendRemovedApprovedPersonNotification(
                        It.Is<AssociatedPersonResults>(result => 
                            result.TemplateId == _messagingConfig.RemovedApprovedUserTemplateId), 
                        It.IsAny<string>()),
                    Times.Exactly(1));
            _mockMessagingService
                .Verify(x => x.SendRemovedApprovedPersonNotification(
                        It.Is<AssociatedPersonResults>(result => 
                            result.TemplateId == _messagingConfig.PromotedApprovedUserTemplateId), 
                        It.IsAny<string>()),
                    Times.Exactly(1));
        }

        [TestMethod]
        public async Task ValidRequest_AddRemoveApprovedUser_OKResult()
        {
            var token = "someToken";
            // Arrange
            var request = new FacadeAddRemoveApprovedPersonRequest
            {
                InvitedPersonEmail = "test@test.com",
                InvitedPersonFirstName = "FirstName",
                InvitedPersonLastName = "LastName",
                OrganisationId = Guid.NewGuid()
            };

            _mockRegulatorOrganisationService
                .Setup(x => x.AddRemoveApprovedUser(It.IsAny<AddRemoveApprovedUserRequest>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new AddRemoveApprovedPersonResponseModel { InviteToken = token }))
                });

            _mockMessagingService
                .Setup(x => x.SendEmailToInvitedNewApprovedPerson(It.IsAny<AddRemoveNewApprovedPersonEmailModel>()));
            // Act
            var result = await _sut.AddRemoveApprovedUser(request);
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
        
        [TestMethod]
        public async Task ValidRequest_AddRemoveApprovedUser_ShouldSendEmailToInvitedNewApprovedPerson()
        {
            var token = "someToken";
            // Arrange
            var request = new FacadeAddRemoveApprovedPersonRequest
            {
                InvitedPersonEmail = "test@test.com",
                InvitedPersonFirstName = "FirstName",
                InvitedPersonLastName = "LastName",
                OrganisationId = Guid.NewGuid()
            };

            _mockRegulatorOrganisationService
                .Setup(x => x.AddRemoveApprovedUser(It.IsAny<AddRemoveApprovedUserRequest>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new AddRemoveApprovedPersonResponseModel { InviteToken = token }))
                });

            _mockMessagingService
                .Setup(x => x.SendEmailToInvitedNewApprovedPerson(It.IsAny<AddRemoveNewApprovedPersonEmailModel>()));
            // Act
            _ = await _sut.AddRemoveApprovedUser(request);
            
            // Assert
            _mockMessagingService
                .Verify(x => x.SendEmailToInvitedNewApprovedPerson(It.Is<AddRemoveNewApprovedPersonEmailModel>(model => model.InviteLink == $"{_messagingConfig.AccountCreationUrl}{token}")));
        }
        
        [TestMethod]
        public async Task ValidRequest_AddRemoveApprovedUser_ShouldSendEmailToDemotedBasicUsers()
        {
            var token = "someToken";
            // Arrange
            var request = new FacadeAddRemoveApprovedPersonRequest
            {
                InvitedPersonEmail = "test@test.com",
                InvitedPersonFirstName = "FirstName",
                InvitedPersonLastName = "LastName",
                OrganisationId = Guid.NewGuid()
            };

            var addRemoveApprovedUserResponse = new AddRemoveApprovedPersonResponseModel
            {
                InviteToken = token,
                AssociatedPersonList = new List<AssociatedPersonResults>
                {
                    new() { 
                        Email = "demotedPerson1@email.com", 
                        OrganisationId = request.OrganisationId.ToString(), 
                        FirstName = "Test", 
                        LastName = "User",
                        CompanyName = "Test Company",
                        EmailNotificationType = "DemotedDelegatedUsed"
                        
                    },  // 2 demoted users
                    new()
                    { 
                        Email = "demotedPerson2@email.com", 
                        OrganisationId = request.OrganisationId.ToString(), 
                        FirstName = "", 
                        LastName = "",
                        CompanyName = "Test Company 2",
                        EmailNotificationType = "DemotedDelegatedUsed"
                        
                    }
                }
            };
            _mockRegulatorOrganisationService
                .Setup(x => x.AddRemoveApprovedUser(It.IsAny<AddRemoveApprovedUserRequest>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        JsonConvert.SerializeObject(addRemoveApprovedUserResponse))
                });

            _mockMessagingService
                .Setup(x => x.SendEmailToInvitedNewApprovedPerson(It.IsAny<AddRemoveNewApprovedPersonEmailModel>()));
            // Act
            _ = await _sut.AddRemoveApprovedUser(request);
            
            // Assert
            _mockMessagingService
                .Verify(x => x.SendRemovedApprovedPersonNotification(
                        It.IsAny<AssociatedPersonResults>(), 
                        It.IsAny<string>()),
                    Times.Exactly(1));
            var usersWithName = addRemoveApprovedUserResponse.AssociatedPersonList.Where(r => !string.IsNullOrWhiteSpace(r.FirstName) && !string.IsNullOrWhiteSpace(r.LastName)).ToArray<AssociatedPersonResults>();
            foreach (var demotedBasicUser in usersWithName)
            {
                _mockMessagingService
                    .Verify(x => x.SendRemovedApprovedPersonNotification(
                            It.Is<AssociatedPersonResults>(result => 
                                result.TemplateId == _messagingConfig.DemotedDelegatedUserTemplateId
                                && result.Email == demotedBasicUser.Email
                                && result.OrganisationId == demotedBasicUser.OrganisationId), 
                            It.IsAny<string>()),
                        Times.Exactly(1));
            }
            
        }

        [TestMethod]
        public async Task InValidRequest_AddRemoveApprovedUser_BadResult()
        {
            // Arrange
            var request = new FacadeAddRemoveApprovedPersonRequest
            {
                InvitedPersonEmail = "test@test.com",
                InvitedPersonFirstName = "FirstName",
                InvitedPersonLastName = "LastName",
                OrganisationId = Guid.NewGuid()
            };
        
            _mockMessagingService
                .Setup(x => x.SendEmailToInvitedNewApprovedPerson(It.IsAny<AddRemoveNewApprovedPersonEmailModel>()));
            // Act
            var result = await _sut.AddRemoveApprovedUser(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Invalid_RemoveApprovedPerson_Exception_Handled()
        {
            // Arrange
            var request = new RemoveApprovedUsersRequest
            {
                OrganisationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RemovedConnectionExternalId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.Empty
            };

            // Act
            var result = await _sut.RemoveApprovedPerson(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task Invalid_GetProducerOrganisationsUsersByExternalId_Is_Called_And_Request_Is_Valid_Then_Return_BadResult()
        {
            // Arrange
            _ = new List<OrganisationUserOverviewResponseModel>();
            
            // Act
            var result = await _sut.GetUsersByOrganisationExternalId(_organisationExternalId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_User_Is_InValid_Then_Return_Null()
        {
            // Arrange
            var organisations = new OrganisationSearchResult();
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(organisations))
            });

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task On_Getting_Organisation_Details_With_User_Invalid_Result_Can_Be_Null()
        {
            // Arrange
            var companyObject = new OrganisationDetailResults();
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(companyObject))
            });

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task When_GetProducerOrganisationsUsersByExternalId_Is_Called_And_User_Is_InValid_Then_Return_Null()
        {
            // Arrange
            var users = new List<OrganisationUserOverviewResponseModel>();
            _mockRegulatorOrganisationService.Setup(x =>
                x.GetUsersByOrganisationExternalId(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(users))
            });

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.GetUsersByOrganisationExternalId(_organisationExternalId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task When_RemoveApprovedPerson_RemoveWithoutNomination_InValid_User_Result_Is_Null()
        {
            // Arrange
            var connExternalId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            var associatedPerson = new List<AssociatedPersonResults>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "user",
                    Email = "test@user.com",
                    OrganisationId = "12545",
                    CompanyName = "Test Company",
                    ServiceRoleId = 1,
                    EmailNotificationType = "RemovedApprovedUser"
                }
            };

            var request = new RemoveApprovedUsersRequest
            {
                RemovedConnectionExternalId = connExternalId,
                OrganisationId = organisationId,
                UserId = Guid.NewGuid(),
                PromotedPersonExternalId = Guid.Empty
            };

            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(request)
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(associatedPerson))
            });

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.RemoveApprovedPerson(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task InValidRequest_AddRemoveApprovedUser_NullResult()
        {
            var token = "someToken";
            // Arrange
            var request = new FacadeAddRemoveApprovedPersonRequest
            {
                InvitedPersonEmail = "test@test.com",
                InvitedPersonFirstName = "FirstName",
                InvitedPersonLastName = "LastName",
                OrganisationId = Guid.NewGuid()
            };

            _mockRegulatorOrganisationService
                .Setup(x => x.AddRemoveApprovedUser(It.IsAny<AddRemoveApprovedUserRequest>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new AddRemoveApprovedPersonResponseModel { InviteToken = token }))
                });

            _mockMessagingService
                .Setup(x => x.SendEmailToInvitedNewApprovedPerson(It.IsAny<AddRemoveNewApprovedPersonEmailModel>()));

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.AddRemoveApprovedUser(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }

        [TestMethod]
        public async Task InValidServiceRequest_AddRemoveApprovedUser_ReturnNull()
        {
            // Arrange
            var request = new FacadeAddRemoveApprovedPersonRequest
            {
                InvitedPersonEmail = "test@test.com",
                InvitedPersonFirstName = "FirstName",
                InvitedPersonLastName = "LastName",
                OrganisationId = Guid.NewGuid()
            };

            // Act
            var result = await _sut.AddRemoveApprovedUser(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(null);
        }
    }
}