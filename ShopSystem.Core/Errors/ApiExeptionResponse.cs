using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        // Represents additional details or information about the exception
        public string? Details { get; set; }

        // Allows specifying additional details about the exception
        public ApiExceptionResponse(int statusCode, string? message = null, string? details = null) : base(statusCode, message)
        {
            Details = details;
        }
    }
}
