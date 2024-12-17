using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

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

        //Error methods
        public static ServiceResult Error(ProblemDetails problemDetails, HttpStatusCode statusCode)
        {
            return new ServiceResult
            {
                StatusCode = statusCode,
                Fail = problemDetails
            };
        }

        public static ServiceResult Error(string title, HttpStatusCode statusCode)
        {
            return new ServiceResult
            {
                StatusCode = statusCode,

                //burada tekrar statuscode'u almamızın sebebi bir hata olduğunda sadece buradaki fail nesnesini döneceğimiz için elimizde statuscode olmayacak.
                Fail = new ProblemDetails()
                {
                    Title = title,
                    Status = statusCode.GetHashCode()
                }
            };
        }

        public static ServiceResult Error(string title, string description, HttpStatusCode statusCode)
        {
            return new ServiceResult
            {
                StatusCode = statusCode,

                //burada tekrar statuscode'u almamızın sebebi bir hata olduğunda sadece buradaki fail nesnesini döneceğimiz için elimizde statuscode olmayacak.
                Fail = new ProblemDetails()
                {
                    Title = title,
                    Detail = description,
                    Status = statusCode.GetHashCode()
                }
            };
        }

        public static ServiceResult ErrorFromProblemDetails(Refit.ApiException apiException)
        {
            if (string.IsNullOrEmpty(apiException.Content))
            {
                return new ServiceResult
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

            return new ServiceResult
            {
                Fail = problemDetails,
                StatusCode = apiException.StatusCode
            };
        }
        public static ServiceResult ErrorFromValidation(IDictionary<string, object?> errors)
        {
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.BadRequest,
                Fail = new ProblemDetails()
                {
                    Title = "Validation error/errors occured",
                    Detail = "Please check the errors.",
                    Extensions = { { "ValidationErrors", errors } },
                    Status = (int)HttpStatusCode.BadRequest.GetHashCode()
                }
            };
        }

    }

    //buradaki new'lerin sebebi: polimorfizmden kaçınmak (virtual override).
    //ServiceResult<T> oluşutursam alttakiler ServiceResult oluşturursam üsttekiler çalışır.
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
        [JsonIgnore] public string? UrlAsCreated { get; set; }

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
        public static ServiceResult<T> SuccessAsCreated(T data, string url)
        {
            return new ServiceResult<T>
            {
                StatusCode = HttpStatusCode.Created,
                Data = data,
                UrlAsCreated = url
            };
        }

        public new static ServiceResult<T> Error(ProblemDetails problemDetails, HttpStatusCode statusCode)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,
                Fail = problemDetails
            };
        }

        public new static ServiceResult<T> Error(string title, HttpStatusCode statusCode)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,

                //burada tekrar statuscode'u almamızın sebebi bir hata olduğunda sadece buradaki fail nesnesini döneceğimiz için elimizde statuscode olmayacak.
                Fail = new ProblemDetails()
                {
                    Title = title,
                    Status = statusCode.GetHashCode()
                }
            };
        }

        public new static ServiceResult<T> Error(string title, string description, HttpStatusCode statusCode)
        {
            return new ServiceResult<T>
            {
                StatusCode = statusCode,

                //burada tekrar statuscode'u almamızın sebebi bir hata olduğunda sadece buradaki fail nesnesini döneceğimiz için elimizde statuscode olmayacak.
                Fail = new ProblemDetails()
                {
                    Title = title,
                    Detail = description,
                    Status = statusCode.GetHashCode()
                }
            };
        }

        public new static ServiceResult<T> ErrorFromProblemDetails(ApiException apiException)
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
        public new static ServiceResult<T> ErrorFromValidation(IDictionary<string,object?> errors)
        {
            return new ServiceResult<T>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Fail = new ProblemDetails()
                {
                    Title = "Validation error/errors occured",
                    Detail = "Please check the errors.",
                    Extensions = { { "ValidationErrors", errors } },
                    Status = (int)HttpStatusCode.BadRequest.GetHashCode()
                }
            };
        }
    }
}
