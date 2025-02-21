﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopSystem.Core.Errors
{
    public class ApiResponse
    {
        [JsonIgnore]
        public int StatusCode { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
        public ApiResponse()
        {
        }
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
        }

        // Provides default messages for common HTTP status codes
        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "You are not Authorized",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => null,
            };
        }
    }
}
