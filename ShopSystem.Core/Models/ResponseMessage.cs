using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Models
{
    public class ResponseMessage<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Error { get; set; } // Optional, for error messages


        public ResponseMessage(string message, T data, bool success, string error)
        {
            Message = message;
            Data = data;
            Success = success;
            Error = error;

        }
    }
}
