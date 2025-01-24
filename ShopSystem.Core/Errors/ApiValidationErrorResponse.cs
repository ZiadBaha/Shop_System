using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        // Represents a collection of validation errors
        public IEnumerable<string> Errors { get; set; }

        // Initializes the Errors collection as an empty list
        public ApiValidationErrorResponse() : base(400)
        {
            Errors = new List<string>();
        }
    }
}
