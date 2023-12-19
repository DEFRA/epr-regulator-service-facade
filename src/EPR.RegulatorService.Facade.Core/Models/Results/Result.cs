using System.Net;

namespace EPR.RegulatorService.Facade.Core.Models.Results
{
    public class Result<T> where T : class
    {
        public Result(bool isSuccess, T value, string errorMessage, HttpStatusCode statusCode)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public bool IsSuccess { get; }
        public T Value { get; }
        public string ErrorMessage { get; }
        public HttpStatusCode StatusCode { get; }

        public static Result<T> SuccessResult(T value) => new(true, value, null, HttpStatusCode.OK);

        public static Result<T> FailedResult(string errorMessage, HttpStatusCode statusCode)
            => new(false, default, errorMessage, statusCode);
    }
}
