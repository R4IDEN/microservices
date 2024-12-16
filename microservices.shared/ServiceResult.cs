using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace microservices.shared
{
    public class ServiceResult
    {
        [JsonIgnore] //bu property zaten cevap ile dönüyor. Tekrar eklememek içinm ignore ediyoruz.
        public HttpStatusCode StatusCode { get; set; }

        public ProblemDetails? Fail { get; set; }

        [JsonIgnore] public bool isSuccess => Fail is null;
        [JsonIgnore] public bool isFail => !isSuccess;

        //static factory metodlar
        public static ServiceResult SuccessAsNoContext()
        {
            return new ServiceResult { StatusCode = HttpStatusCode.NoContent };
        }

        public static ServiceResult ErrorAsNotFound()
        {
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.NotFound,
                Fail = new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = "The requested source was not found"
                }
            };
        }


    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
        public string? UrlAsCreated { get; set; }

        public static ServiceResult<T> SuccessAsNoContext(T data)
        {
            return new ServiceResult<T> { StatusCode = HttpStatusCode.OK, Data = data };
        }

        public static ServiceResult<T> ErrorAsBadRequest(ProblemDetails problemDetails)
        {
            return new ServiceResult<T>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Fail = problemDetails
            };
        }

        //201 => Created => response body header => location == api/products/5
        public static ServiceResult<T> SuccessAsOK(T data, string url)
        {
            return new ServiceResult<T>
            {
                StatusCode = HttpStatusCode.Created,
                Data = data,
                UrlAsCreated = url
            };
        }

        public static ServiceResult<T> ErrorFromProblemDetails(Refit.ApiException apiException)
        {
            if (string.IsNullOrEmpty(apiException.Content))
            {
                return new ServiceResult<T>
                {
                    Fail = new ProblemDetails()
                    {
                        Title = apiException.Message
                    },
                    StatusCode = apiException.StatusCode
                };
            }

            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(apiException.Content, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            return new ServiceResult<T>
            {
                Fail = problemDetails,
                StatusCode = apiException.StatusCode
            };
        }

        public static ServiceResult<T> Error(ProblemDetails problemDetails, HttpStatusCode statusCode)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,
                Fail = problemDetails
            };
        }

        public static ServiceResult<T> ErrorAsNotFound()
        {
            return new ServiceResult<T>
            {
                StatusCode = HttpStatusCode.NotFound,
                Fail = new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = "The requested source was not found"
                }
            };
        }
    }
}
