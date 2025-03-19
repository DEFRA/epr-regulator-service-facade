using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EPR.RegulatorService.Facade.API.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    public class FileUploadController : ControllerBase
    {
        private readonly IAntivirusService _antivirusService;

        public FileUploadController(
        IAntivirusService antivirusService)
        {
            _antivirusService = antivirusService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("file-upload")]
        public async Task<IActionResult> UploadFile([FromBody] FileUploadRequest request)
        {
            try
            {
                var fileStream = new MemoryStream(request.FileContent);
                var userId = User.UserId();
                var email = User.Email();

                var antiVirusResponse = await _antivirusService.SendFile(request.SubmissionType, request.FileId, request.FileName, fileStream, userId, email);
                var objectResult = new ObjectResult(antiVirusResponse) { StatusCode = (int)antiVirusResponse.StatusCode };

                return objectResult;
            }
            catch (Exception ex) 
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            
        }
    }
}
